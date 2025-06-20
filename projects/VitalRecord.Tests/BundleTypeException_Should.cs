using System;
using System.Threading.Tasks;
using Xunit;

namespace VitalRecord.Tests
{
    public class BundleTypeException_Should
    {
        [Fact]
        public void DefaultConstructor_ShouldCreateExceptionWithDefaultMessage()
        {
            var exception = new BundleTypeException();

            Assert.NotNull(exception);
            Assert.IsType<BundleTypeException>(exception);
            Assert.IsAssignableFrom<Exception>(exception);
            Assert.NotNull(exception.Message);
            Assert.NotEmpty(exception.Message);
        }

        [Fact]
        public void MessageConstructor_ShouldCreateExceptionWithSpecifiedMessage()
        {
            var expectedMessage = "Custom bundle type exception message";
            var exception = new BundleTypeException(expectedMessage);

            Assert.NotNull(exception);
            Assert.IsType<BundleTypeException>(exception);
            Assert.IsAssignableFrom<Exception>(exception);
            Assert.Equal(expectedMessage, exception.Message);
        }

        [Fact]
        public void MessageConstructor_WithEmptyMessage_ShouldCreateExceptionWithEmptyMessage()
        {
            var expectedMessage = "";
            var exception = new BundleTypeException(expectedMessage);

            Assert.NotNull(exception);
            Assert.IsType<BundleTypeException>(exception);
            Assert.IsAssignableFrom<Exception>(exception);
            Assert.Equal(expectedMessage, exception.Message);
        }

        [Fact]
        public void MessageAndInnerExceptionConstructor_ShouldCreateExceptionWithMessageAndInnerException()
        {
            var expectedMessage = "Bundle type exception with inner exception";
            var innerException = new ArgumentException("Inner exception message");
            var exception = new BundleTypeException(expectedMessage, innerException);

            Assert.NotNull(exception);
            Assert.IsType<BundleTypeException>(exception);
            Assert.IsAssignableFrom<Exception>(exception);
            Assert.Equal(expectedMessage, exception.Message);
            Assert.Equal(innerException, exception.InnerException);
        }

        [Fact]
        public void MessageAndInnerExceptionConstructor_WithNullInnerException_ShouldCreateExceptionWithNullInnerException()
        {
            var expectedMessage = "Bundle type exception with null inner exception";
            var exception = new BundleTypeException(expectedMessage, null);

            Assert.NotNull(exception);
            Assert.IsType<BundleTypeException>(exception);
            Assert.IsAssignableFrom<Exception>(exception);
            Assert.Equal(expectedMessage, exception.Message);
            Assert.Null(exception.InnerException);
        }

        [Fact]
        public async Task Exception_ShouldBeThrowableAndCatchable()
        {
            var expectedMessage = "Test exception message";

            var thrownException = await Assert.ThrowsAsync<BundleTypeException>(() =>
            {
                throw new BundleTypeException(expectedMessage);
            });
            Assert.Equal(expectedMessage, thrownException.Message);
        }

        [Fact]
        public void Exception_WithInnerException_ShouldPreserveInnerExceptionDetails()
        {
            var innerMessage = "Inner exception occurred";
            var outerMessage = "Bundle type exception occurred";
            var innerException = new ArgumentNullException("paramName", innerMessage);
            var exception = new BundleTypeException(outerMessage, innerException);

            Assert.Equal(outerMessage, exception.Message);
            Assert.Equal(innerException, exception.InnerException);
            Assert.IsType<ArgumentNullException>(exception.InnerException);
            Assert.Contains(innerMessage, exception.InnerException.Message);
        }

        [Fact]
        public void Exception_ShouldHaveCorrectToStringOutput()
        {
            var message = "Test bundle type exception";
            var exception = new BundleTypeException(message);
            var result = exception.ToString();

            Assert.Contains("BundleTypeException", result);
            Assert.Contains(message, result);
        }

        [Fact]
        public void Exception_WithInnerException_ShouldIncludeInnerExceptionInToString()
        {
            var outerMessage = "Outer exception";
            var innerMessage = "Inner exception";
            var innerException = new InvalidOperationException(innerMessage);
            var exception = new BundleTypeException(outerMessage, innerException);
            var result = exception.ToString();

            Assert.Contains("BundleTypeException", result);
            Assert.Contains(outerMessage, result);
            Assert.Contains("InvalidOperationException", result);
            Assert.Contains(innerMessage, result);
        }
    }
}
