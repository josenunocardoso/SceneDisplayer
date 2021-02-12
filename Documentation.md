This file intends to give a technical overview of the behavior of the classes of this library, and to demonstrate some examples of its use.

## Technical overview

![High-level class diagram](https://raw.githubusercontent.com/josenunocardoso/SceneDisplayer/master/documentation/entities.png)

- A project that uses this library must interact with the SceneManager, in order to control what is drawn into the screen.

- A SceneManager can contain multiple Scenes, but only one of them - the ActiveScene - can be shown at the same time. We can think of a Scene the same way we think about slides in a slideshow presentation. We can define multiple Scenes and then control when and how which one is being shown.

- A Scene can contain multiple Entities. An Entity represents a subset of the Scene to be drawn. This can be a basic shape (like Circles, Lines, Rectangles) or other kind of objects, like text or images.

- Any Entity may contain other Entities, as children. This allows to extend the already defined Entities from this Library, by creating new Entities that contain multiple children. This concept is very useful to avoid code repetition and to abstract complex objects.

## Examples

#### Draw a Rectangle

```
class Program
{
    static void Main(string[] args)
    {
        SceneManager.Init(new TestScene(), "Hello World");
        SceneManager.Render();
        SceneManager.Dispose();
    }
}

public class TestScene : Scene
{
    public TestScene()
    {
        var rectangle = new FillRectangle(
            new RectF(0.5f, 0.5f, 0.25f, 0.25f), Color.Red
        );
        this.AddEntity(rectangle);
    }
}
```

- In this excert of code, we define a filled rectangle - FillRectangle, by passing the location and its size as the first argument, and a color as the second argument.

- The position and size given are values in the range of [0, 1], where 1 represents the width or the height of the screen, for the X and Y coordinates, respectively.

- Lastly we add the Entity to the Scene.

- This is the result:

![FillRectangle Example](https://raw.githubusercontent.com/josenunocardoso/SceneDisplayer/master/documentation/FillRectangleExample.png)

#### Create an Entity with a child

```
public class MyEntity : Entity
{
    public override void Init()
    {
        var redRectangle = new FillRectangle(
            new RectF(0.5f, 0.5f, 0.25f, 0.25f), Color.Red
        );
        var greenRectangle = new FillRectangle(
            new RectF(0.5f, 0.6f, 0.15f, 0.1f), Color.Green
        );

        this.AddChild("Red", redRectangle);
        this.AddChild("Green", greenRectangle);
    }
}
```

- Here we created a new Entity, called MyEntity. We also have overriten the Init method, to add two children Entities.

- The AddChild method requests a unique key, alongside the Entity to add. We can later get a given child through their key.

- The order on which we add the children matters. The children that are added first, are rendered first. Therefor, in this case, the green rectangle will be drawn over the red rectangle.

- Finally, we replace the code of the TestScene, to add this Entity, instead of the original FillRectangle.

```
public class TestScene : Scene
{
    public TestScene()
    {
        var myEntity = new MyEntity();
        this.AddEntity(myEntity);
    }
}
```

![Children Entity Example](https://raw.githubusercontent.com/josenunocardoso/SceneDisplayer/master/documentation/ChildrenEntityExample.png)

## TODO
