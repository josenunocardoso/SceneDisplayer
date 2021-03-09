This file intends to give a technical overview of the behavior of the classes of this library, and each one of the defined Entities.

## Technical overview

![High-level class diagram](https://raw.githubusercontent.com/josenunocardoso/SceneDisplayer/master/documentation/entities.png)

- A project that uses this library must interact with the SceneManager, in order to control what is drawn into the screen.

- A SceneManager can contain multiple Scenes, but only one of them - the ActiveScene - can be shown at the same time. We can think of a Scene the same way we think about slides in a slideshow presentation. We can define multiple Scenes and then control when and how which one is being shown.

- A Scene can contain multiple Entities. An Entity represents a subset of the Scene to be drawn. This can be a basic shape (like Circles, Lines, Rectangles) or other kind of objects, like text or images.

- Any Entity may contain other Entities, as children. This allows to extend the already defined Entities from this Library, by creating new Entities that contain multiple children. This concept is very useful to avoid code repetition and to abstract complex objects.

## Entities

### Table of Contents

- [Rectangle](#rectangle)
- [FillRectangle](#fillrectangle)
- [Image](#image)


## Rectangle

## FillRectangle

## Image
