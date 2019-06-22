using System;
using Xunit;
using FluentAssertions;

namespace Api.Tests
{
    public class SampleTests
    {
        [Fact]
        public void Test_Should_Run()
        {
            var expected = 1;
            var actual = 1;
            actual.Should().Be(expected);
        }
    }
}
