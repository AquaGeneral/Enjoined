# Enjoined - Better 3D Physics Joint Editor UI for Unity

> Enjoined is in an alpha state, and may be unstable and/or incomplete. Note that Enjoined does not support the new (Unity 2017.2 and newer) [WYSIWYG Joint Angular Limit Handles](https://docs.unity3d.com/2017.2/Documentation/ScriptReference/IMGUI.Controls.JointAngularLimitHandle.html).

Enjoined sets out to simplify and streamline the 3D joint editors in Unity.

[<img src="https://github.com/AquaGeneral/Enjoined/raw/images/EnjoinedConfigurableJointTeaser.png" width="906" height="658"/>](https://github.com/AquaGeneral/Enjoined/raw/images/EnjoinedConfigurableJoint.png)

[![](https://github.com/AquaGeneral/Enjoined/raw/images/EnjoinedConfigurableJointTeaser.png =906x658)](https://github.com/AquaGeneral/Enjoined/raw/images/EnjoinedConfigurableJoint.png)

Enjoined fixes all of the following things:
* All fields are displayed as a formatted version of their API name, which might be okay in a programming environment they aren't always descriptive or indicative enough.
* Joint limits sometimes refer to "rotational limits" in degrees, or "positional limits" in units (meters if you're using PhysX),
* There are many toggles that sit in their own line, while they could easily be reformatted into tidier toggles with the parmaters as a title/header,
* Quaternions are displayed across 5 lines, when they can easily be shrunken down to one line,
* Things like "X Motion", "Y Motion", "Z Motion" are all on seperate lines and can easily and readably be packed into one line,
* The "Spring" parameter really means the "Spring Force",
* The "Linear X/Y/Z Limit" is not inherently descriptive enough in simple English, instead "Displacement/Positional Limit" is more suitable.
* The foldouts for all serialized classes/structs such as Spring Limits are unncessary and are the source of some clutter.
* The Configurable Joint inspector is gargantuan,
* Configurable Joint's "low/high" angular joint limits should be called "lower/upper" instead,
* Configurable Joint's "low/high" angular joint limits are seperate and make it somewhat confusing to jump between the two - they could easily be inlined,
* Hinge Joint has a helpbox stating that the Limit (angle) should be fit within the range of -180 to 180 instead of just enforcing this directly.
* ..there's probably several other things I forgot to mention...
