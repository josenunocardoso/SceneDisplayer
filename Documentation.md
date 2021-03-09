This file intends to give a technical overview of the behavior of the classes of this library, and each one of the defined Entities and utility classes.

### Table of Contents

- [Technical overview](#technical-overview)
- [Entities](#entities)
  - [Entity](#entity)
  - [RectangularEntity](#rectangularentity)
  - [Rectangle](#rectangle)
  - [FillRectangle](#fillrectangle)
  - [Image](#image)
  - [TextEntity](#textentity)
  - [Circle](#circle)
  - [Line](#line)
  - [VisibleWrapper](#visiblewrapper)
- [Utils](#utils)
  - [RectF](#rectf)
  - [PointF](#pointf)
  - [Color](#color)

## Technical overview

![High-level class diagram](https://raw.githubusercontent.com/josenunocardoso/SceneDisplayer/master/documentation/entities.png)

- A project that uses this library must interact with the SceneManager, in order to control what is drawn into the screen.

- A SceneManager can contain multiple Scenes, but only one of them - the ActiveScene - can be shown at the same time. We can think of a Scene the same way we think about slides in a slideshow presentation. We can define multiple Scenes and then control when and how which one is being shown.

- A Scene can contain multiple Entities. An Entity represents a subset of the Scene to be drawn. This can be a basic shape (like Circles, Lines, Rectangles) or other kind of objects, like text or images.

- Any Entity may contain other Entities, as children. This allows to extend the already defined Entities from this Library, by creating new Entities that contain multiple children. This concept is very useful to avoid code repetition and to abstract complex objects.

## Entities


### Entity

#### Description
A component that is displayed on a Scene. It may contain other Entities as children.

#### Properties

- <b>Key</b>: The Key that was given with AddChild.
  - Type: <b>object</b>
- <b>RelativeToScreenSize</b>: True, to consider positions relative to the screen size. False, to consider absolute positions, in pixels.
  - Type: <b>bool</b>
  - By default is <b>true</b>.
  - If this property set to <b>true</b>, the coordinates (0.5, 0.5) would represent the center of the window.
  - If this property is set to <b>false</b>, the coordinates (200, 300) would represent a point 200px away from the left side of the window, and 300px away from the top side of the window.
- <b>Children</b>: The children of this Entity.

#### Main Methods

- <b>AddChild</b>: Adds an Entity as a new child, and initializes it.
  - key: Key of the child.
    - Type: <b>object</b>
    - The key must not be null, and must be unique. This key is then used to identify this Entity.
  - child: Entity to be added.
    - Type: <b>Entity</b>

- <b>RemoveChild</b>: Removes a child and disposes it.
  - key: The key of the child to be removed.
    - Type: <b>object</b>
    - There must be a child with the given key.

- <b>PerformActionOnChildrenRecursively</b>: Performs an action on all the children. This action is performed recursively (e.g. on the children of the children).
  - ac: The action to perform.
    - Type: <b>Action\<Entity\></b>

- <b>PerformActionOnChildrenRecursively</b>: Performs an action on all the children on a given predicate. This action is performed recursively (e.g. on the children of the children).
  - ac: The action to perform.
    - Type: <b>Action\<Entity\></b>
  - pred: The predicate to test.
    - Type: <b>Predicate\<Entity\></b>

- <b>ClearChildren</b>: Clears all the children, and disposes them.

- <b>Init</b>: Initializes the Entity. It is called when an Entity is added.
  - Empty virtual method; can be overriden by sub-classes of Entity.

- <b>Draw</b>: Draws the Entity. It is called by the SceneManager.Render method. The method is called once, for every frame rendered.
  - Virtual method, that calls recursively the Draw method on its children. base.Draw must be called at the beginning, if this method is overriden.
  - renderer: Pointer to the renderer used.
    - Type: <b>IntPtr</b>
  - windowWidth: Window width in pixels.
    - Type: <b>int</b>
  - windowHeight: Window height in pixels.
    - Type: <b>int</b>
  - deltaTime: Time elapsed since the last draw call, in milliseconds.
    - Type: <b>uint</b>

- <b>Dispose</b>: Disposes the Entity.
  - Virtual method, that calls recursively the Dispose method on its children. base.Dispose must be called at the beginning, if this method is overriden.


### RectangularEntity

#### Description
An [<b>Entity</b>](#entity) that has a rectangular shape.

#### Properties

- <b>Area</b>: The area of the Rectangle.
  - Type: [<b>RectF</b>](#rectf)
  - The x and y coordinates represent the position of the center of the Rectangle. w and h represent the width and the height respectively.
- [<b>RelativeToScreenSize</b>](#entity)

#### Main methods

- <b>OnClick</b>: Method called by the SceneManager, when there is a click inside of the Area defined on this Entity.


### Rectangle

#### Description
An [<b>Entity</b>](#entity) that renders a rectangle.

#### Properties

- <b>Color</b>: The color of the Rectangle.
  - Type: [<b>Color</b>](#color)
- [<b>Area</b>](#rectangularentity)
- [<b>RelativeToScreenSize</b>](#entity)


### FillRectangle

#### Description
An [<b>Entity</b>](#entity) that renders a filled rectangle.

#### Properties

- <b>Color</b>: The color of the Rectangle.
  - Type: [<b>Color</b>](#color)
- [<b>Area</b>](#rectangularentity)
- [<b>RelativeToScreenSize</b>](#rectangularentity)


### Image

#### Description
An [<b>Entity</b>](#entity) that renders an image.

#### Properties

- <b>ImagePath</b>: The image path.
  - Type: <b>string</b>
- <b>Alpha</b>: The image alpha channel. Set it to 0x0 to make the image transparent. Set it to 0xFF to make it opaque.
  - Type: <b>byte</b>
- <b>Angle</b>: The angle in degrees of the rotation applied on the center of the image.
  - Type: <b>double</b>.
- [<b>Area</b>](#rectangularentity)
- [<b>RelativeToScreenSize</b>](#entity)

#### Behavior
- This Entity uses cache, shared between all the Images. If an Image instance has set the same ImagePath as another instance, that image will be shared between both of them.
- If the given ImagePath is being used for the first time, a texture for the image will be created on the fly, on the first Draw method call.


### TextEntity

#### Description
An [<b>Entity</b>](#entity) that renders text.

#### Properties

- <b>Text</b>: The text to be rendered.
  - Type: <b>string</b>
- <b>Font</b>: The font path.
  - Type: <b>string</b>
- <b>FontSize</b>: The font size.
  - Type: <b>int</b>
- <b>TextColor</b>: The text color.
  - Type: [<b>Color</b>](#color)
- <b>Location</b>: The location of the Entity.
  - Type: [<b>PointF</b>](#pointf)
- [<b>RelativeToScreenSize</b>](#entity)

#### Main Methods

- <b>GetTextSize</b>: Retrieves the size a given TextEntity template would occupy.
  - text: Text.
    - Type: <b>string</b>
  - font: Font path.
    - Type: <b>string</b>
  - fontSize: Font size.
    - Type: <b>int</b>
  - textColor: Text color.
    - Type: [<b>Color</b>](#color)
  - <b>Return Type: (int, int)</b>: A tuple with the width and the height of the template, respectively.

- <b>GetTextAbsoluteLocation</b>: Returns the absolute location of this TextEntity, according to the TextAlignment used.
  - alignment: The text alignment.
    - Type: <b>string</b>
  - area: The area where the TextEntity would be placed.
    - Type: <b>SDL_Rect</b>
  - <b>Return Type: [PointF](#pointf)</b>

#### Behavior
- This Entity uses cache, shared between all the TextEntites. If an Image instance has set the same Text, Font, FontSize and Color as another instance, that text will be shared between both of them.
- If the given Text, Font, FontSize or Color are being used for the first time, a texture for the text will be created on the fly, on the first Draw method call.

### Circle

#### Description
An Entity that renders a circle.

#### Properties

- <b>Center</b>: The center of the Circle.
  - Type: [<b>PointF</b>](#pointf)
- <b>Radius</b>: The radius of the Circle.
  - Type: <b>float</b>
- <b>Color</b>: The color of the Circle.
  - Type: [<b>Color</b>](#color)
- <b>Sides</b>: The number of sides of the Circle.
  - Type: <b>int</b>
  - By default is <b>10</b>.
  - The more sides it has, the more it looks like a Circle. However, the performance decreases the bigger the number of sides is.
- [<b>RelativeToScreenSize</b>](#entity)

#### Main methods

- <b>OnClick</b>: Method called by the SceneManager, when there is a click inside of the Circle defined by the center and the radius in this Entity.

### Line

#### Description
An Entity that renders a line.

### Properties

- <b>Source</b>: The source point of the Line.
  - Type: [<b>PointF</b>](#pointf)
- <b>Target</b>: The destination point of the Line.
  - Type: [<b>PointF</b>](#pointf)
- <b>Color</b>: The color of the Line.
  - Type: [<b>Color</b>](#color)
- [<b>RelativeToScreenSize</b>](#entity)

### VisibleWrapper

#### Description
An Entity that encapsulates another Entity and manages whether that Entity is visible or not.

#### Properties

- <b>Visible</b>: Whether the child is set to be rendered, or not.
  - Type: <b>bool</b>
- <b>Entity</b>: The encapsulated Entity.
  - Type: [<b>Entity</b>](#entity)


## Utils


### RectF

#### Description
Represents a floated rectangle.

#### Fields

- x: X position.
  - Type: <b>float</b>.
- y: Y position.
  - Type: <b>float</b>.
- w: Width.
  - Type: <b>float</b>.
- h: Height.
  - Type: <b>float</b>.

#### Main Methods

- <b>ToSDLRect</b>: Returns an equivalent SDL_Rect, with the values casted to int.
  - <b>Return Type: SDL_Rect</b>

- <b>Contains</b>: Returns whether this RectF contains a given point.
  - point: The point to compare.
    - Type: <b>SDL_Point</b>
  - <b>Return Type: bool</b>: True if it contains, False otherwise.

- <b>Contains</b>: Returns whether this RectF contains a given point.
  - point: The point to compare.
    - Type: [<b>PointF</b>](#pointf)
  - <b>Return Type: bool</b>: True if it contains, False otherwise.

### PointF

#### Description
Represents a floated point.

#### Fields

- x: X position.
  - Type: <b>float</b>.
- y: Y position.
  - Type: <b>float</b>.


### Color

#### Description
Represents a RGBA color.

#### Fields

- r: Red channel.
  - Type: <b>byte</b>.
  - Range: 0x00 (0) to 0xFF (255)
- g: Green channel.
  - Type: <b>byte</b>.
  - Range: 0x00 (0) to 0xFF (255)
- b: Blue channel.
  - Type: <b>byte</b>.
  - Range: 0x00 (0) to 0xFF (255)
- a: Alpha channel.
  - Type: <b>byte</b>.
  - By default is <b>0xFF</b>.
  - Range: 0x00 (0) to 0xFF (255)

#### Defined Colors (static fields)

- <span style="color:transparent">Transparent</span>
- <span style="color:cyan">Cyan</span>
- <span style="color:black">Black</span>
- <span style="color:blue">Blue</span>
- Magenta
- Grey
- Green
- Lime
- Maroon
- NavyBlue
- Olive
- Purple
- Red
- Silver
- Teal
- White
- Yellow
- Orange
