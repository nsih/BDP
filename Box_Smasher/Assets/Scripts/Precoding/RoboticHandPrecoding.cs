using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
알고리즘 구상 (ctw0727)

- 천장과 연결되는 부위를 좌표A, 두 관절이 연결되는 부위를 좌표B, 모니터의 위치를 좌표C로 둔다
- A와 C를 중심으로 하고 관절의 두 길이를 반지름으로 하는 두 원의 교점을 구하는 방식으로 좌표B를 구함
- AB와 BC의 중점에 각각 해당하는 관절을 배치하고 각도를 조정

+++ 좌우반전 시 부드럽게 이동하는 스크립트 있으면 좋을 거 같지만 일단 따로 생각해보기
*/

public class RoboticHandPrecoding : MonoBehaviour
{
    private Vector3 PosStart, PosAnkle, PosDef;		// 시작지점, 관절위치, 목표지점
	private float StoA, AtoD;						// 관절의 길이
	
	// Pos1에서 Pos2로 향하는 각도를 반환
	private Quaternion Get_Pos_rotation(Vector3 Pos1, Vector3 Pos2){
        
        float angle = Mathf.Atan2(Pos2.y-Pos1.y, Pos2.x-Pos1.x) * Mathf.Rad2Deg;
        
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        
        return rotation;
    }
	
	// 입력받은 두 좌표간의 거리를 반환
    float Get_Vector3_Range(Vector3 Pos1, Vector3 Pos2){
		
        return Mathf.Sqrt(Mathf.Pow((Pos1.x - Pos2.x),2)+Mathf.Pow((Pos1.y - Pos2.y),2));
    }
	
	// PosAnkle의 좌표를 구하는 함수
	private Vector3 Get_Pos_Ankle(Vector3 P1, Vector3 P2, bool Lefted){
		
		float Length = Get_Vector3_Range(P1, P2);
		float buf1 = Mathf.Acos(( Mathf.Pow(StoA, 2) - Mathf.Pow(AtoD, 2) + Mathf.Pow(Length, 2) ) / ( 2 * StoA * Length));
		float buf2 = Mathf.Atan( (P2.y - P1.y) / (P2.x - P1.x) );
		
		if (Lefted == false){
			float buf3 = P1.x + StoA * Mathf.cos(buf2 + buf1);
			float buf4 = P1.y + StoA * Mathf.sin(buf2 + buf1);
			
		}
		else{
			float buf3 = P1.x + StoA * Mathf.cos(buf2 - buf1);
			float buf4 = P1.y + StoA * Mathf.sin(buf2 - buf1);
		}
		
		Vector3 retPosition = new Vector3(buf3, buf4, 0f);
		return retPosition;
	}
	
	// 두 좌표의 중심점을 반환하는 함수
	private Vector3 get_Middle_Pos(Vector3 P1, Vector3 P2){
		
	}
	
	// 천장과 가까운 관절의 좌표를 반환하는 함수
	public Vector3 get_Pos_AnkleA(bool Lefted){
		PosAnkle = Get_Pos_Ankle(PosStart, PosDef, Lefted);
		float X = PosAnkle.x - PosStart.x;
		float Y = PosAnkle.y - PosStart.y;
		X = X/2;
		Y = Y/2;
		Vector3 RET = new Vector3(PosStart.x + X, PosStart.y + Y, 0);
		return RET;
	}
	
	// 보스와 가까운 관절의 좌표를 반환하는 함수
	public Vector3 get_Pos_AnkleB(bool Lefted){
		PosAnkle = Get_Pos_Ankle(PosStart, PosDef, Lefted);
		float X = PosDef.x - PosAnkle.x;
		float Y = PosDef.y - PosAnkle.y;
		X = X/2;
		Y = Y/2;
		Vector3 RET = new Vector3(PosAnkle.x + X, PosAnkle.y + Y, 0);
		return RET;
	}
	
	// 천장과 가까운 관절의 각도를 반환하는 함수
	public Quaternion get_Quaternion_AnkleA(bool Lefted){
		PosAnkle = Get_Pos_Ankle(PosStart, PosDef, Lefted);
		return Get_Pos_rotation(PosStart, PosAnkle);
	}
	
	// 보스와 가까운 관절의 각도를 반환하는 함수
	public Quaternion get_Quaternion_AnkleB(bool Lefted){
		PosAnkle = Get_Pos_Ankle(PosStart, PosDef, Lefted);
		return Get_Pos_rotation(PosAnkle, PosDef);
	}
