using UnityEngine;

public class StoryNPCSpawn : MonoBehaviour
{
    public static StoryNPCSpawn Instance;

    [SerializeField] private GameObject NPCpasseggero;
    [SerializeField] private GameObject NPCcappellaioRivelazione;

    [SerializeField] private Transform[] spawnPointsItems;
    [SerializeField] private Transform spawnPointCappello;

    private void Awake() {
        if (Instance != null) { // Se c'e' gia' istanza 
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SpawnNPC(ItemType itemType) {
        if(itemType == ItemType.Cappello) {
            Vector3 spawnPosition = spawnPointCappello.transform.position;
            
            GameObject npc = Instantiate(NPCcappellaioRivelazione, spawnPosition, Quaternion.identity);
            Vector3 direction = Player.Instance.transform.position - npc.transform.position;
            direction.y = 0f;

            npc.transform.rotation = Quaternion.LookRotation(direction);
        }
        else {
            int index = (int)itemType; // si utilizza itemtype come indice dello spawn, quindi nell'inspector
            // bisogna metterli nell'ordine corretto
            Vector3 spawnPosition = spawnPointsItems[index].transform.position;
            GameObject npc = Instantiate(NPCpasseggero, spawnPosition, Quaternion.identity);
            Vector3 direction = Player.Instance.transform.position - npc.transform.position;
            direction.y = 0f;

            npc.transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
