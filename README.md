# Moq.Extensions
Расширение для библиотеки [Moq](https://github.com/moq/moq4), позволяющее создавать макеты классов, подменяющие поведение невиртуальных свойств.


![nuget version](https://img.shields.io/nuget/v/Const-Phi.Moq.Extensions)
![nuget dowmloads counter](https://img.shields.io/nuget/dt/Const-Phi.Moq.Extensions)


## Ограничения
### Работа с интерфесами
Предположим, что в иерархии типов, для которых предполагается использовать библиотеку, существует такая подиерархия
```cs
public interface IEntity
{
    int Id { get; set; }
}

public sealed class User : IEntity
{
  public int Id { get; set; }
  
  public string Login { get; set; }
}
```

И для быстрого создания макетов сущностей было написано некоторое количество построителей ([builder](https://refactoring.guru/ru/design-patterns/builder))  
```cs
public abstract class EntityTestDataBuilder<TEntity, TBuilder> : BaseTestDataBuilder<TEntity, TBuilder>
    where TEntity : class, IEntity
    where TBuilder : EntityTestDataBuilder<TEntity, TBuilder>
{
    public TBuilder WithId(int id)
    {
        return this.RegisterValueForProperty(x => x.Id, id);
    }
}

public sealed class UserTestDataBuilder : EntityTestDataBuilder<User, UserTestDataBuilder>
{  
    public TBuilder WithLogin(string login)
    {
        return this.RegisterValueForProperty(x => x.Login, login);
    }
}
```

Для описанного выше случая в текущей версии библиотеки существует важное ограничение, для свойства `Login` значение присваиваться будет, а для `Id` &ndash; нет, так как оно задаётся интерфесом `IEntity`, а не классом `User`.

```cs
using NUnit.Framework;

[TestFeature]
public class UserTestDataBuilderTests
{
    [Test]
    public void Test()
    {
        // arrange
        var expectedId = 1;
        
        var expectedLogin = "test login";
        
        // act
        var user = new UserTestDataBuilder()
            .With(targerId)
            .WithLogin(targetLogin)
            .Build();
            
        // assert
        Assert.IsNotNull(user);
        Assert.AreEqual(targetLogin, user.Login);
        Assert.AreEqual(targetId, user.Id); // fail here
    }
}
```
