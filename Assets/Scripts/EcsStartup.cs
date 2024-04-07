using System;
using Dune.IO;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using NaughtyAttributes;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Dune.IO {
    sealed class EcsStartup : MonoBehaviour {
        
        [field: BoxGroup("Configuration")]
        [field: SerializeField]
        public Configuration Configuration { get; private set; }
        
        [field: BoxGroup("Joystick")]
        [field: SerializeField]
        public FixedJoystick FixedJoystick { get; private set; }
        
        [field: Foldout("Canvas")]
        [field: SerializeField]
        public Button RestartButton { get; private set; }
        
        [field: Foldout("Canvas")]
        [field: SerializeField]
        public TMP_Text ScoreText { get; private set; }
        
        [field: Foldout("Canvas")]
        [field: SerializeField]
        public Button StartButton { get; private set; }
        
        [field: Foldout("Canvas")]
        [field: SerializeField]
        public Button BuyHarvesterButton { get; private set; }
        
        [field: Foldout("Canvas")]
        [field: SerializeField]
        public GameObject StartPanel { get; private set; }
        
        [field: Foldout("Canvas")]
        [field: SerializeField]
        public Button LandingButton { get; private set; }
        
        [field: Foldout("Canvas")]
        [field: SerializeField]
        public Button FireButton { get; private set; }
        
        EcsWorld _world;        
        IEcsSystems _updateSystems;
        IEcsSystems _fixedUpdateSystems;

        void Start () {
            
            _world = new EcsWorld ();
            _updateSystems = new EcsSystems (_world, Configuration);
            _fixedUpdateSystems = new EcsSystems (_world, Configuration);
            
            // Services
            var scoreService = new ScoreService(Configuration);
            var pointerService = new PointerService(_world).Init(this);
            var uiService = new UiService(_world, 
                    RestartButton, 
                    ScoreText,
                    StartButton, 
                    BuyHarvesterButton, 
                    FireButton,
                    LandingButton,
                    StartPanel, 
                    Configuration,
                    scoreService)
                .Init(this);
            
            //Update systems
            _updateSystems
                //Harvester systems   
                .Add(new BuyHarvesterSystem(scoreService))
                .Add(new HarvesterMovingSystem())
                .Add(new HarvesterOutlineSystem())
                .Add(new HarvesterMiningSystem())
                //Factory systems
                .Add(new UploadingSpice(scoreService))
#if UNITY_EDITOR
                // add debug systems for custom worlds here, for example:
                // .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem ("events"))
                .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem ())
#endif
                .Inject()
                .Init ();

            //Fixed update systems
            _fixedUpdateSystems
                // Ornithopter systems
                .Add(new OrnithopterInitSystem())
                .Add(new OrnithopterMovingSystem(FixedJoystick))
                .Add(new OrnithopterLandingSystem())
                .Inject()
                .Init ();
        }

        private void Update () {
            // process systems here.
            _updateSystems?.Run ();
        }

        private void FixedUpdate()
        {
            _fixedUpdateSystems?.Run();
        }

        void OnDestroy () {
            if (_updateSystems != null) {
                // list of custom worlds will be cleared
                // during IEcsSystems.Destroy(). so, you
                // need to save it here if you need.
                _updateSystems.Destroy ();
                _updateSystems = null;
            }
            
            if (_fixedUpdateSystems != null) {
                // list of custom worlds will be cleared
                // during IEcsSystems.Destroy(). so, you
                // need to save it here if you need.
                _fixedUpdateSystems.Destroy ();
                _fixedUpdateSystems = null;
            }
            
            // cleanup custom worlds here.
            
            // cleanup default world.
            if (_world != null) {
                _world.Destroy ();
                _world = null;
            }
        }
    }
}