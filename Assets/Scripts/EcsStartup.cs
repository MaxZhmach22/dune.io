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
        
        EcsWorld _world;        
        IEcsSystems _systems;

        void Start () {
            
            var scoreService = new ScoreService(Configuration);
            
            _world = new EcsWorld ();
            _systems = new EcsSystems (_world, Configuration);
            _systems
                .Add(new BuyHarvesterSystem(scoreService))
                .Add(new HarvesterMovingSystem())
                .Add(new HarvesterOutlineSystem())
                // register your systems here, for example:
                // .Add (new TestSystem1 ())
                // .Add (new TestSystem2 ())
                
                // register additional worlds here, for example:
                // .AddWorld (new EcsWorld (), "events")
#if UNITY_EDITOR
                // add debug systems for custom worlds here, for example:
                // .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem ("events"))
                .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem ())
#endif
                .Inject()
                .Init ();
            
            // Create pointer events subscription
            var pointerService = new PointerService(_world).Init(this);
            var uiService = new UiService(_world, RestartButton, ScoreText, StartButton, BuyHarvesterButton, StartPanel, Configuration, scoreService)
                .Init(this);
        }

        void Update () {
            // process systems here.
            _systems?.Run ();
        }

        void OnDestroy () {
            if (_systems != null) {
                // list of custom worlds will be cleared
                // during IEcsSystems.Destroy(). so, you
                // need to save it here if you need.
                _systems.Destroy ();
                _systems = null;
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