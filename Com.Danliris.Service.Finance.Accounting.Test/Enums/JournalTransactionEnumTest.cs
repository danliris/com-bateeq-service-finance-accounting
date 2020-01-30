using Com.Danliris.Service.Finance.Accounting.Lib.Enums.JournalTransaction;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Enums
{
    public class JournalTransactionEnumTest
    {
        [Fact]
        public void Should_Success_Return_Division_Spinning()
        {
            var result = JournalNumberGenerator.GetDivisionByNumber(1);
            Assert.Equal("S", result);
        }

        [Fact]
        public void Should_Success_Return_Division_Weaving()
        {
            var result = JournalNumberGenerator.GetDivisionByNumber(2);
            Assert.Equal("W", result);
        }

        [Fact]
        public void Should_Success_Return_Division_FinishingPrinting()
        {
            var result = JournalNumberGenerator.GetDivisionByNumber(3);
            Assert.Equal("F", result);
        }

        [Fact]
        public void Should_Success_Return_Division_GarmentPurchasing()
        {
            var result = JournalNumberGenerator.GetDivisionByNumber(4);
            Assert.Equal("G", result);
        }

        [Fact]
        public void Should_Success_Return_Division_Umum()
        {
            var result = JournalNumberGenerator.GetDivisionByNumber(5);
            Assert.Equal("U", result);
        }

        [Fact]
        public void Should_Success_Return_Draft()
        {
            var result = JournalTransactionStatus.Draft;
            Assert.Equal("DRAFT", result);
        }

        [Fact]
        public void Should_Success_Return_Posted()
        {
            var result = JournalTransactionStatus.Posted;
            Assert.Equal("POSTED", result);
        }
    }
}
