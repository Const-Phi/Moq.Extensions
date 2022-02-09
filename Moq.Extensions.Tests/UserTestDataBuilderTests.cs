namespace Moq.Extensions.Tests
{
    using NUnit.Framework;

    /// <summary>
    /// Non-sealed target type.
    /// </summary>
    public class User
    {
        public string LastName { get; set; }

        public string FirstName { get; set; }

        public int? NullableValue { get; set; }
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
            const string targetLastName = "LastName";

            const string targetFirstName = "FirstName";

            const int targetValue = 42;

            // act
            var user = new UserTestDataBuilder()
                .WithLastName(targetLastName)
                .WithFirstName(targetFirstName)
                .WithNullableValue(targetValue)
                .Build();

            // assert
            Assert.IsNotNull(user);
            Assert.AreEqual(targetLastName, user.LastName);
            Assert.AreEqual(targetFirstName, user.FirstName);
            Assert.AreEqual(targetValue, user.NullableValue);
        }
    }
}
