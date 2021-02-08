# SceneDisplayer

The Scene Displayer Library is built on top of [SDL](https://www.sdl.com/). It allows to easily render scenes that contain 2D objects in C#. Since it is built on top of [SDL](https://www.sdl.com/), the library intends to be able to target Windows, MacOS and Linux.

The main purpose of this project is to make it simple to render multiple scenes, by abstracting some of the SDL rendering logic.

Anything that renders something on a scene is called an Entity. There are multiple Entities already defined on this library, like basic shapes (rectangles, lines, circles), text or images.

The library allows to mix the already defined Entities, to form new Entities, which makes it simple to define different kinds of UI objects.

The library is intended to be used for:

- 2D Graphical applications developed relatively quickly and easily.
- 2D Graphical applications that use mostly the Entities defined by the library, or a mix of them.
- 2D Games.
- Simple slideshows.
- Applications developed in C#, for Desktop.

The library is not intended to be used for:

- Complex graphical applications, with different kinds of UI elements. Although it is possible to do more complex applications with this library, you most likely would have to define multiple Entities before-hand.
- Classic desktop applications, that use mostly IO elements (e.g. TextBoxes, ComboBoxes, etc.).
- 3D Graphical applications

## Dependencies

TODO

## Examples

TODO
