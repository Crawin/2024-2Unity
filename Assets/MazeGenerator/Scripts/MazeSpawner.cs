using UnityEngine;
using System.Collections;
using Unity.AI.Navigation;

//<summary>
//Game object, that creates maze and instantiates it in scene
//</summary>
public class MazeSpawner : MonoBehaviour {
	public enum MazeGenerationAlgorithm{
		PureRecursive,
		RecursiveTree,
		RandomTree,
		OldestTree,
		RecursiveDivision,
	}

	public MazeGenerationAlgorithm Algorithm = MazeGenerationAlgorithm.PureRecursive;
	public bool FullRandom = false;
	public int RandomSeed = 12345;
	public GameObject Floor = null;
	public GameObject Wall = null;
	public GameObject Pillar = null;
	public int Rows = 5;
	public int Columns = 5;
	public float CellWidth = 5;
	public float CellHeight = 5;
	public bool AddGaps = true;
	public GameObject GoalPrefab = null;
	public GameObject ChomperPrefab = null;
	public int nChompers = 5;
	public GameObject SpitterPrefab = null;
    public int nSpitters = 5;
    public GameObject GrenadierPrefab = null;
    public int nGrenadiers = 1;
	public NavMeshSurface NavMeshSurface = null;
	private int m_iLeftTotalMobs = 0;
	private int[] m_iLeftMobs;
    private BasicMazeGenerator mMazeGenerator = null;

	void Start () {
        m_iLeftTotalMobs = nChompers + nSpitters + nGrenadiers;
		m_iLeftMobs = new int[3];
		m_iLeftMobs[0] = nChompers;
		m_iLeftMobs[1] = nSpitters;
		m_iLeftMobs[2] = nGrenadiers;
        if (!FullRandom) {
			Random.seed = RandomSeed;
		}
		switch (Algorithm) {
		case MazeGenerationAlgorithm.PureRecursive:
			mMazeGenerator = new RecursiveMazeGenerator (Rows, Columns);
			break;
		case MazeGenerationAlgorithm.RecursiveTree:
			mMazeGenerator = new RecursiveTreeMazeGenerator (Rows, Columns);
			break;
		case MazeGenerationAlgorithm.RandomTree:
			mMazeGenerator = new RandomTreeMazeGenerator (Rows, Columns);
			break;
		case MazeGenerationAlgorithm.OldestTree:
			mMazeGenerator = new OldestTreeMazeGenerator (Rows, Columns);
			break;
		case MazeGenerationAlgorithm.RecursiveDivision:
			mMazeGenerator = new DivisionMazeGenerator (Rows, Columns);
			break;
		}
		mMazeGenerator.GenerateMaze ();
		for (int row = 0; row < Rows; row++) {
			for(int column = 0; column < Columns; column++){
				float x = column*(CellWidth+(AddGaps?.2f:0));
				float z = row*(CellHeight+(AddGaps?.2f:0));
				MazeCell cell = mMazeGenerator.GetMazeCell(row,column);
				GameObject tmp;
				tmp = Instantiate(Floor,new Vector3(x,0,z), Quaternion.Euler(0,0,0)) as GameObject;
				tmp.transform.parent = transform;
				if(cell.WallRight){
					tmp = Instantiate(Wall,new Vector3(x+CellWidth/2,0,z)+Wall.transform.position,Quaternion.Euler(0,90,0)) as GameObject;// right
					tmp.transform.parent = transform;
				}
				if(cell.WallFront){
					tmp = Instantiate(Wall,new Vector3(x,0,z+CellHeight/2)+Wall.transform.position,Quaternion.Euler(0,0,0)) as GameObject;// front
					tmp.transform.parent = transform;
				}
				if(cell.WallLeft){
					tmp = Instantiate(Wall,new Vector3(x-CellWidth/2,0,z)+Wall.transform.position,Quaternion.Euler(0,270,0)) as GameObject;// left
					tmp.transform.parent = transform;
				}
				if(cell.WallBack){
					tmp = Instantiate(Wall,new Vector3(x,0,z-CellHeight/2)+Wall.transform.position,Quaternion.Euler(0,180,0)) as GameObject;// back
					tmp.transform.parent = transform;
				}
				if(cell.IsGoal && GoalPrefab != null){
					tmp = Instantiate(GoalPrefab,new Vector3(x,1,z), Quaternion.Euler(0,0,0)) as GameObject;
					tmp.transform.parent = transform;
				}
				//if (cell.IsGoal && m_iLeftTotalMobs > 0)
				//{
				//	int type;
				//	while (true)
				//	{
    //                    type = Random.Range(0, 3);
    //                    if (m_iLeftMobs[type] > 0)
				//		{
				//			--m_iLeftMobs[type];
    //                        break;
				//		}
				//	}
				//	switch (type)
				//	{
				//		case 0:
    //                        tmp = Instantiate(ChomperPrefab, new Vector3(x, 1, z), Quaternion.Euler(0, 0, 0)) as GameObject;
    //                        break;
				//		case 1:
    //                        tmp = Instantiate(SpitterPrefab, new Vector3(x, 1, z), Quaternion.Euler(0, 0, 0)) as GameObject;
    //                        break;
				//		case 2:
    //                        tmp = Instantiate(GrenadierPrefab, new Vector3(x, 1, z), Quaternion.Euler(0, 0, 0)) as GameObject;
    //                        break;
				//	}
    //                tmp.transform.parent = transform;
    //                --m_iLeftTotalMobs;
    //            }
			}
		}
		if(Pillar != null){
			for (int row = 0; row < Rows+1; row++) {
				for (int column = 0; column < Columns+1; column++) {
					float x = column*(CellWidth+(AddGaps?.2f:0));
					float z = row*(CellHeight+(AddGaps?.2f:0));
					GameObject tmp = Instantiate(Pillar,new Vector3(x-CellWidth/2,0,z-CellHeight/2),Quaternion.identity) as GameObject;
					tmp.transform.parent = transform;
				}
			}
		}
		NavMeshSurface.BuildNavMesh();

        for (int row = Rows-1; row >= 0; --row)
        {
            for (int column = Columns-1; column >= 0; --column)
            {
                float x = column * (CellWidth + (AddGaps ? .2f : 0));
                float z = row * (CellHeight + (AddGaps ? .2f : 0));
                MazeCell cell = mMazeGenerator.GetMazeCell(row, column);
                GameObject tmp;
                if (cell.IsGoal && m_iLeftTotalMobs > 0)
                {
                    int type;
                    while (true)
                    {
                        type = Random.Range(0, 3);
                        if (m_iLeftMobs[type] > 0)
                        {
                            --m_iLeftMobs[type];
                            break;
                        }
                    }
                    switch (type)
                    {
                        case 0:
                            tmp = Instantiate(ChomperPrefab, new Vector3(x, 1, z), Quaternion.Euler(0, 0, 0)) as GameObject;
                            tmp.transform.parent = transform;
                            break;
                        case 1:
                            tmp = Instantiate(SpitterPrefab, new Vector3(x, 1, z), Quaternion.Euler(0, 0, 0)) as GameObject;
                            tmp.transform.parent = transform;
                            break;
                        case 2:
                            tmp = Instantiate(GrenadierPrefab, new Vector3(x, 1, z), Quaternion.Euler(0, 0, 0)) as GameObject;
                            tmp.transform.parent = transform;
                            break;
                    }
                    --m_iLeftTotalMobs;
                }
            }
        }
    }
}
