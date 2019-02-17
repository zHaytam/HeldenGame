using System;
using System.Collections.Generic;
using Assets.Scripts.I18n;
using NUnit.Framework;

namespace Assets.Scripts.Tests
{
    public class CsvReaderTests
    {

        [Test]
        public void ParseToList_ShouldParseSimpleRow()
        {
            string text = "Col1,Col2,Col3,Col4";
            var rows = CsvReader.ParseToList(text, 4, false, row => row);

            string[] expected = { "Col1", "Col2", "Col3", "Col4" };
            Assert.That(rows[0], Is.EquivalentTo(expected));
        }

        [Test]
        public void ParseToList_ShouldParseMultipleRows()
        {
            string text = "Col1_1,Col1_2,Col1_3\nCol2_1,Col2_2,Col2_3";
            var rows = CsvReader.ParseToList(text, 3, false, row => row);

            var expected = new List<string[]>
            {
                new[] {"Col1_1", "Col1_2", "Col1_3"},
                new[] {"Col2_1", "Col2_2", "Col2_3"}
            };

            Assert.That(rows, Is.EquivalentTo(expected));
        }

        [Test]
        public void ParseToList_ShouldHandleEmptyColumns()
        {
            string text = "Col1,,";
            var rows = CsvReader.ParseToList(text, 3, false, row => row);

            Assert.AreEqual(3, rows[0].Length);
        }

        [Test]
        public void ParseToList_ShouldReplaceEmptyColumnsWithNull()
        {
            string text = "Col1,,";
            var rows = CsvReader.ParseToList(text, 3, false, row => row);

            Assert.AreEqual(3, rows[0].Length);
        }

        [Test]
        public void ParseToList_ShouldThrowWhenReceivingMoreColumnsThanExpected()
        {
            string text = "Col1,Col2,";
            var ex = Assert.Throws<Exception>(() => CsvReader.ParseToList(text, 2, false, row => row));
            Assert.That(ex.Message, Is.EqualTo("Line 1 contains more than 2 columns."));
        }

        [Test]
        public void ParseToList_ShouldThrowWhenReceivingLessColumnsThanExpected()
        {
            string text = "Col1,Col2";
            var ex = Assert.Throws<Exception>(() => CsvReader.ParseToList(text, 3, false, row => row));
            Assert.That(ex.Message, Is.EqualTo("Line 1 doesn't contain 3 columns."));
        }

        [Test]
        public void ParseToList_ShouldThrowWhenMissingClosingDoubleQuotes()
        {
            string text = "Col1,\"Col2";
            var ex = Assert.Throws<Exception>(() => CsvReader.ParseToList(text, 2, false, row => row));
            Assert.That(ex.Message, Is.EqualTo("Missing closing double quotes in line 1."));
        }

        [Test]
        public void ParseToList_ShouldHandleCommasInsideDoubleQuotes()
        {
            string text = "Col1,\"Col, 2\",Col3";
            var rows = CsvReader.ParseToList(text, 3, false, row => row);

            Assert.AreEqual(3, rows[0].Length);
        }

        [Test]
        public void ParseToList_ShouldHandleDoubleQuotesInTheLastColumn()
        {
            string text = "Col1,\"Col, 2\",\"Col3\"";
            var rows = CsvReader.ParseToList(text, 3, false, row => row);

            Assert.AreEqual(3, rows[0].Length);
        }

        [Test]
        public void ParseToList_ShouldHandleNewlinesInsideColumns()
        {
            string text = "Something,\"Line 1.\r\nLine 2.\"";
            var rows = CsvReader.ParseToList(text, 2, false, row => row);

            string[] expected = { "Something", "Line 1.\r\nLine 2." };
            Assert.That(rows[0], Is.EquivalentTo(expected));
        }

        [Test]
        public void ParseToList_ShouldIgnoreHeaderWhenAskedTo()
        {
            string text = "Key,Value\nName,Alex";
            var rows = CsvReader.ParseToList(text, 2, true, row => row);

            Assert.AreEqual(1, rows.Count);
        }

        [Test]
        public void ParseToList_ShouldHandleDoubleQuotesEscaping()
        {
            string text = "Test,\"Test \"\"1\"\"\"";
            var rows = CsvReader.ParseToList(text, 2, false, row => row);

            string[] expected = { "Test", "Test \"1\"" };
            Assert.That(rows[0], Is.EquivalentTo(expected));
        }

    }
}
