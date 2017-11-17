# Enjoined - Better 3D Physics Joint Editor UI for Unity

> Note that Enjoined does not support the new (Unity 2017.2 and newer) [angular limit WYSIWIG editors](https://docs.unity3d.com/2017.2/Documentation/ScriptReference/IMGUI.Controls.JointAngularLimitHandle.html) and is not at all finished -expect to find bugs and accidental ommisions.

Enjoined sets out to simplify and streamline the 3D joint editors in Unity.

Enjoined fixes all of the following things:
* Joint limits sometimes refer to "rotational limits" in degrees, or "positional limits" in units (meters if you're using PhysX),
* There are many toggles that sit in their own line, while they could easily be reformatted into tidier toggles with the parmaters as a title/header,
* Quaternions are displayed across 5 lines, when they can easily be shrunken down to one line,
* Things like "X Motion", "Y Motion", "Z Motion" are all on seperate lines and can easily and readably be packed into one line,
* The "Spring" parameter really means the "Spring Force",
* The "Linear X/Y/Z Limit" parameters really means the "Displacement/Positional Limit"
* The foldouts for all serialized classes/structs such as Spring Limits are unncessary and are the source of some clutter.
* The Configurable Joint inspector is gargantuan,
* Configurable Joint's "low/high" angular joint limits should be called "lower/upper" instead,
* Configurable Joint's "low/high" angular joint limits are seperate and make it somewhat confusing to jump between the two - they could easily be inlined,
* 