﻿using System;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [ObjectSystem]
    public class OperaComponentAwakeSystem : AwakeSystem<OperaComponent>
    {
	    public override void Awake(OperaComponent self)
	    {
		    self.Awake();
	    }
    }

	[ObjectSystem]
	public class OperaComponentUpdateSystem : UpdateSystem<OperaComponent>
	{
		public override void Update(OperaComponent self)
		{
			self.Update();
		}
	}

	public class OperaComponent: Component
    {
        public Vector3 ClickPoint;

	    public int mapMask;

	    public void Awake()
	    {
		    this.mapMask = LayerMask.GetMask("Map");
	    }

	    private readonly Frame_ClickMap frameClickMap = new Frame_ClickMap();

        public void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
	            if (Physics.Raycast(ray, out hit, 1000, this.mapMask))
	            {
					this.ClickPoint = hit.point;
		            frameClickMap.X = this.ClickPoint.x;
		            frameClickMap.Y = this.ClickPoint.y;
		            frameClickMap.Z = this.ClickPoint.z;
		            ETModel.SessionComponent.Instance.Session.Send(frameClickMap);

					// 测试actor rpc消息
					this.TestActor().Coroutine();
				}
            }


            if (Input.GetKeyDown(KeyCode.Q))
            {
	            CastQSkill();
            }
        }

        private static void CastQSkill()
        {
	        var  myPlayer = ETModel.Game.Scene.GetComponent<PlayerComponent>().MyPlayer;
	        var playerUnit = ETModel.Game.Scene.GetComponent<UnitComponent>().Get(myPlayer.UnitId);
	        var pos =playerUnit.Position;
	        var rot = playerUnit.Rotation;
	        UnityEngine.GameObject go = UnityEngine.GameObject.CreatePrimitive(PrimitiveType.Cube);
			go.transform.SetPositionAndRotation(pos,rot);
			UnityEngine.GameObject.Destroy(go,3);


			Frame_WaveAttack attack=new Frame_WaveAttack();
			attack.ActorId = myPlayer.UnitId;
			attack.YAngle = (int)playerUnit.Rotation.y;
			ETModel.SessionComponent.Instance.Session.Send(attack);
        }

        public async ETVoid TestActor()
	    {
		    try
		    {
			    M2C_TestActorResponse response = (M2C_TestActorResponse)await SessionComponent.Instance.Session.Call(
						new C2M_TestActorRequest() { Info = "actor rpc request" });
			    Log.Info(response.Info);
			}
		    catch (Exception e)
		    {
				Log.Error(e);
		    }
		}
    }
}
