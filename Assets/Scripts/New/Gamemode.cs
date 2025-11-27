using UnityEngine;

public abstract class Gamemode : MonoBehaviour
{
    public abstract void OnItemSelected(Collider2D[] items);
    public abstract void Return();
}
