namespace Moq.Extensions.Tests
{
    using System.Collections.Generic;
    using NUnit.Framework;

    /// <summary>
    /// Non-sealed target type.
    /// </summary>
    public class User
    {
        public string LastName { get; set; }

        public string FirstName { get; set; }

        public int? NullableValue { get; set; }

        public Tag Tag { get; set; }

        public IList<Tag> Tags { get; set; }
    }

    public class Tag
    {
        public virtual string Value { get; set; }
    }

    public class ProxyTag : Tag
    {
    }


    /// <summary>
    /// Target type test data builder.
    /// </summary>
    public class UserTestDataBuilder : BaseTestDataBuilder<User, UserTestDataBuilder>
    {
        public UserTestDataBuilder WithLastName(string lastName)
        {
            return this.RegisterValueForProperty(x => x.LastName, lastName);
        }

        public UserTestDataBuilder WithFirstName(string firstName)
        {
            return this.RegisterValueForProperty(x => x.FirstName, firstName);
        }

        public UserTestDataBuilder WithNullableValue(int value)
        {
            return this.RegisterValueForProperty(x => x.NullableValue, value);
        }

        public UserTestDataBuilder WithTag(Tag tag)
        {
            return this.RegisterValueForProperty(x => x.Tag, tag);
        }

        public UserTestDataBuilder WithTags(List<Tag> tags)
        {
            return this.RegisterValueForProperty(x => x.Tags, tags);
        }
    }

    [TestFixture]
    public class UserTestDataBuilderTests
    {
        [Test]
        public void ComplexMockedObject_Success()
        {
            // arrange
            const string targetLastName = "LastName";

            const string targetFirstName = "FirstName";

            // act
            var user = new UserTestDataBuilder()
                .WithLastName(targetLastName)
                .WithFirstName(targetFirstName)
                .Build();

            // assert
            Assert.IsNotNull(user);
            Assert.AreEqual(targetLastName, user.LastName);
            Assert.AreEqual(targetFirstName, user.FirstName);
        }

        [Test]
        public void ComplexMockedObject_Nullable_Success()
        {
            // arrange
            const int targetValue = 42;

            // act
            var user = new UserTestDataBuilder()
                .WithNullableValue(targetValue)
                .Build();

            // assert
            Assert.IsNotNull(user);
            Assert.AreEqual(targetValue, user.NullableValue);
        }

        [Test]
        public void ComplexMockedObject_Inheritance_Success()
        {
            // arrange
            var targetValue = "target value";

            var proxyTag = new ProxyTag { Value = targetValue };

            // act
            var user = new UserTestDataBuilder()
                .WithTag(proxyTag)
                .Build();

            // assert
            Assert.IsNotNull(user);
            Assert.IsNotNull(user.Tag);
            Assert.AreEqual(targetValue, user.Tag.Value);
        }

        [Test]
        public void ComplexMockedObject_Interface_Success()
        {
            // arrange
            var targetValue = "target value";

            var tags = new List<Tag> { new Tag { Value = targetValue } };

            // act
            var user = new UserTestDataBuilder()
                .WithTags(tags)
                .Build();

            // assert
            Assert.IsNotNull(user);
            CollectionAssert.AreEqual(user.Tags, tags);
        }
    }
}
