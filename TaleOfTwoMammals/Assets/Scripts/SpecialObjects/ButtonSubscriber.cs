using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//An interface for things which use buttons, so buttons can call the pressed() and unpressed()
//  functions.
public interface ButtonSubscriber
{
    void Pressed(GameObject button);

    void Unpressed(GameObject button);
}
