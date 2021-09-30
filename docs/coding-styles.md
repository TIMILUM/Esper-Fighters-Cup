# {Esper Fighters Cup} Coding Styles

- [C# Code rules](#c-code-rules)
- [Formatting rules](#formatting-rules)
- [Naming rules](#naming-rules)
- [Unity rules](#unity-rules)
- [Editorconfig](#editorconfig)
- [Editorconfig for Rider IDE](#editorconfig-for-rider-ide)

## C# Code rules

- Tab 대신 4개의 Space를 사용합니다.

- `this.` 키워드를 사용하지 않습니다.

```cs
private int _sum;

private void Sum(int num)
{
    // this._sum += num; // X
    _sum += num;
}
```

- `String`, `Int32`와 같은 타입 이름 대신 `string`, `int`와 같은 키워드를 사용합니다.

```cs
// String str1 = "str1"; // X
string str2 = "str2";
```

- 모든 class, method, field, property 등에 접근한정자를 표시합니다.

```cs
// class SomeClass // X
internal class SomeClass // O
{
    // string _someField; // X
    private string _someField; // O

    // string SomeProperty { get; set; } // X
    private string SomeProperty { get; set; } // O

    // void SomeMethod() { } // X
    private void SomeMethod() { } // O
}
```

- 필요한 경우가 아니라면 auto property를 사용합니다.

```cs
// X
// private int _someProperty;
// public int SomeProperty { get { return _someProperty; } }

// O
public int SomeProperty { get; }
```

- 한줄 메소드에 식 본문을 사용하지 않습니다. (다른 메소드와의 통일성을 해침)  
  예외: 로컬 함수, 람다식, 프로퍼티, 기타 등등...

```cs
// X
// private void Start() => Debug.Log("...");

// O
private void Start()
{
    Debug.Log("...");
}
```

- 가능한 경우 `var` 키워드를 사용합니다.  
  단, `string`, `int`, `double`, `bool` 같은 기본 타입의 경우 `var`를 사용하지 않아도 됩니다.

```cs
var str1 = "Hello";
string str2 = "World";

var somePosition = new Vector3(x, y, z);
var newObject = Instantiate(somePrefab, somePosition, Quaternion.identity);

foreach (var item in list)
{
    // ...
}
```

## Formatting rules

- [Allman style](http://en.wikipedia.org/wiki/Indent_style#Allman_style)의 들여쓰기를 사용합니다.

```cs
public class HelloWorldClass
{
    public void HelloWorld()
    {
        //...
        if ()
        {
            //...
        }
    }
}
```

## Naming rules

- 상수는 PascalCase를 사용합니다.

```cs
public const float Pi = 3.14159;
public const string GameName = "Esper Fighters Cup";
```

- private 정적 변수는 `s_`를 앞에 붙입니다.

```cs
private static int s_someField;
```

- 필드는 `_`를 앞에 붙입니다.

```cs
private int _hp;
```

- 이외에는 PascalCase를 사용합니다. (예외 제보바랍니다)

```cs
public class SomeClass { }

public struct SomeStruct { }

public int SomeProperty {get;set;}

public void SomeMethod() { }
```

## Unity rules

- 인스펙터에 표시되어야 하는 변수는 `public`으로 선언하지 않고 `[SerializeField]`를 사용합니다.

```cs
[SerializeField] private float _myField;
```

- Attribute는 필드 선언과 같은 줄에, 여러 개의 경우 `,`로 나열합니다.

```cs
[SerializeField, Min(1.5f)] private float _myField;
```

- 다른 스크립트와 공유하는 데이터의 경우 field 대신 property를 사용합니다.

```cs
public class SomeScript : Monobehaviour
{
    // public int age; // X
    public int Age { get; set; } // O
}
```

(추가 작성 필요)

## Editorconfig

위의 모든 내용들은 프로젝트에 [.editorconfig](./.editorconfig) 파일을 넣는 것으로 자동화가 가능합니다.

## Editorconfig for Rider IDE

Rider IDE를 사용 시 `File → Manage IDE Settings → Import Settings...`을 통해 [settings.zip](./settings.zip) 파일을 불러오는 것으로 자동화가 가능합니다.
