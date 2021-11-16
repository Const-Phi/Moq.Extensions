# Moq.Extensions
Расширение для библиотеки [Moq](https://github.com/moq/moq4), позволяющее создавать макеты классов, подменяющие поведение невиртуальных свойств.


![nuget version](https://img.shields.io/nuget/v/Const-Phi.Moq.Extensions)
![nuget dowmloads counter](https://img.shields.io/nuget/dt/Const-Phi.Moq.Extensions)


## Ограничения
### Работа с интерфесами
Предположим, что в иерархии типов, для которых предполагается использовать библиотеку есть такая подыерархия
```cs
public interface IEntity
{
    int Id { get; }
}

public sealed class User : IEntity
{
  public int Id { get; protected set; }
  
  public string Login { get; protected set; }
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
