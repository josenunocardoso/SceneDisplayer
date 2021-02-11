# SceneDisplayer

The Scene Displayer Library is built on top of [SDL](https://www.libsdl.org/). It allows to easily render scenes that contain 2D objects in C# .Net. Since it is built on top of [SDL](https://www.libsdl.org/), the library intends to be able to target Windows, MacOS and Linux.

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

[.Net 5.0 SDK](https://dotnet.microsoft.com/download)

#### SDL2 Runtime Binaries
- [SDL2](https://www.libsdl.org/download-2.0.php)
- [SDL2_ttf](https://www.libsdl.org/projects/SDL_ttf/)
- [SDL2_image](https://www.libsdl.org/projects/SDL_image/)

## Getting started

- Create a new console application:
  <details>
    <summary>With Visual Studio</summary>

    Create a new Project -> Console App
  </details>

  <details>
    <summary>With CLI</summary>

    ```
    dotnet new console
    ```

  </details>

 - Install the [Nuget package](https://www.nuget.org/packages/Cardoso.SceneDisplayer/)
 
    - Alternatively, you can clone this repo, and reference SceneDisplayer.csproj to your project.
    
    ```
    dotnet add reference <path-to-the-cloned-repo>/SceneDisplayer.csproj
    ```
 
 - Add the SDL2 binaries to the build:
  
    - Create a folder on the project folder that was created. In this example we will call the folder "lib". If you choose a different name, you must replace "lib" with the name you chose, on the following steps.
  
    - Copy all the SDL2 Runtime Binaries to this folder.

    - Automatically copy those binaries to the output folder, after build time:
      <details>
        <summary>With Visual Studio</summary>

        Go to Project Properties -> Build Events

        On "Post-build event command line" add the following line:

        ```
        copy "$(SolutionDir)\lib\*" "$(TargetDir)"
        ```
      </details>

      <details>
        <summary>With CLI</summary>

        Edit the <Project-name>.csproj file.

        Inside the Project tag, add the following:

        ```
        <Target Name="PostBuild" AfterTargets="PostBuildEvent">
          <Exec Command="copy &quot;$(SolutionDir)\lib\*&quot; &quot;$(TargetDir)&quot;" />
        </Target>
        ```

      </details>
      
#### Hello World

- In order to display something on the screen, we must define a Scene and initialize it with the SceneManager.

- The following code should display an empty blue window:
    
```
using SceneDisplayer;

namespace Test
{
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
    }
}
```

- We defined a Scene, called TestScene, and created an instance of it to be passed on the SceneManager.Init method, alongside with the window title "Hello World".

- SceneManager.Render is a blocking method, that does all the rendering logic, and listens to events.

- Finally, SceneManager.Dispose is called when the application is closed, in order to release the used resources.

- For additional information on how to add Entities and how to interact between them and the Scenes, read the [documentation](Documentation.md).
