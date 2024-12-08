using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

namespace SimpleTestSeries.Tests
{
    public class AutoMoqDataAttribute : AutoDataAttribute
    {
        public AutoMoqDataAttribute()
          : base(CreateFixture)
        {
        }

        private static IFixture CreateFixture()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            fixture.Customize<DateOnly>(c => c.FromFactory((DateTime dt) => DateOnly.FromDateTime(dt)));

            return fixture;
        }
    }
}