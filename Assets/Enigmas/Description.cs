using UnityEngine;

[CreateAssetMenu(fileName = "Description", menuName = "Scriptable Objects/Description")]
public class Description : ScriptableObject
{
    /*
        1)
        Name : Ghost's Lair
        Difficulty : Medium      
        The room is in the dark, there is a table in the middle with a flashlight on it. Once the player picks it up, a
        ghost will appear and fly around the room, and will disappear from time to time. While the ghost is invisible,
        the Support can keep track of his position.
        The ghost will take damage if the player points his flashlight at him. Once the ghost has lost enough health,
        he will drop a key, allowing the player to unlock the next room.
        However, the player will die if he doesn't kill the ghost within 2 minutes or so.

        2)
        Name : Mirror
        Difficulty : Easy
        A series of shapes will appear on the terminal.
        The room is divided into two parts. Both parts of the room are mirrored versions of each other.
        There is only one difference : one room will have various shapes scattered around the room, whereas in
        the mirrored room, the shapes will be replaced by numbers. As a result, each shape corresponds to a number.
        The Support can now decipher the given code.
        The room will mostly be in the dark, the lights will flicker from time to time. Some anomalies may be added
        to slow down the Agent's progress.

        3)
        Name : Library
        Difficulty : Medium
        An anomaly will patrol around the room. The Agent needs to pick up notes around the room, where pieces of code
        will be written. Some bookshelves can be moved around by the Support, allowing him to slow down the anomaly's 
        progress.

        4)
        Name : Code Red
        Difficulty : Medium
        The Agent will be equipped with an infrared camera, allowing the Support to follow his point of view.
        There are pieces of code hidden on the walls, on the furniture, inside drawers etc. which can only be seen
        through the camera. Some anomalies will roam around the room to slow down the playe's progress.
    */
}
