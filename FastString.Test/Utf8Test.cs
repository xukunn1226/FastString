﻿using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace FastString.Test
{
	[TestFixture]
	public class Utf8Test
	{
		#region Length
		[Test]
		public void AsciiLength()
		{
			Assert.That(new utf8("hello world").Length, Is.EqualTo(11));
		}

		[Test]
		public void MultibyteCharsLength()
		{
			Assert.That(new utf8("“hello world”").Length, Is.EqualTo(17));
		}

		[Test]
		public void MultibyteCharsBytes()
		{
			var list = new utf8("“hello”").Bytes;
			Assert.That(list, Is.EqualTo(new List<byte> {
				0xe2, 0x80, 0x9c,
				0x68,
				0x65,
				0x6c,
				0x6c,
				0x6f,
				0xe2, 0x80, 0x9d
			}));
		}
		#endregion Length

		#region Iteration
		[Test]
		public void IterateAsciiRange()
		{
			uint[] expected = { 0x68, 0x65, 0x6c, 0x6c, 0x6f };
			var it = new utf8("hello").GetEnumerator();
			for (int i = 0; i < 5; i++)
			{
				Assert.IsTrue(it.MoveNext());
				Assert.That(it.Current.Index, Is.EqualTo(i));
				Assert.That(it.Current.Value, Is.EqualTo(expected[i]));
			}
		}

		[Test]
		public void IterateMultibyteUtf8ButSingleByteUtf16()
		{
			var str = new utf8("“hat”");
			var it = str.GetEnumerator();
			Assert.IsTrue(it.MoveNext());
			Assert.That(it.Current.Value, Is.EqualTo(0x201c));
			Assert.IsTrue(it.MoveNext());
			Assert.That(it.Current.Value, Is.EqualTo(0x68));
			Assert.IsTrue(it.MoveNext());
			Assert.That(it.Current.Value, Is.EqualTo(0x61));
			Assert.IsTrue(it.MoveNext());
			Assert.That(it.Current.Value, Is.EqualTo(0x74));
			Assert.IsTrue(it.MoveNext());
			Assert.That(it.Current.Value, Is.EqualTo(0x201d));
			Assert.IsFalse(it.MoveNext());
		}
		#endregion Iteration

		#region Equality
		[Test]
		public void StringEqualsAscii()
		{
			Assert.That(new utf8("hello") == "hello", Is.True);
			Assert.That(new utf8("hello") == "hellp", Is.False);
		}

		[Test]
		public void StringEqualsParanoiaSc()
		{
			Assert.That(new utf8("Sc") == "Sc", Is.True);
		}

		[Test]
		public void StringEqualsMultibyte()
		{
			Assert.That(new utf8("☃hello") == "☃hello", Is.True);
			Assert.That(new utf8("☃hello") == "☄hello", Is.False);
		}

		[Test]
		public void StringEqualsEmoji()
		{
			Assert.That(new utf8("\ud83d\udca9hello") == "\ud83d\udca9hello", Is.True);
			Assert.That(new utf8("\ud83d\udca9hello") == "\ud83d\udcaahello", Is.False);
		}

		[Test]
		public void Utf8Equals()
		{
			Assert.That(new utf8("\ud83d\udca9hello☂") == new utf8("\ud83d\udca9hello☂"), Is.True);
			Assert.That(new utf8("\ud83d\udca9hello") == new utf8("\ud83d\udcaahello"), Is.False);
			Assert.That(new utf8("\ud83d\udca9hello☂") == new utf8("\ud83d\udca9hello☄"), Is.False);
		}
		#endregion Equality

		[Test]
		public void FromInt()
		{
			Assert.That(utf8.FromInt(0), Is.EqualTo(new utf8("0")));
			Assert.That(utf8.FromInt(1), Is.EqualTo(new utf8("1")));
			Assert.That(utf8.FromInt(10), Is.EqualTo(new utf8("10")));
			Assert.That(utf8.FromInt(12), Is.EqualTo(new utf8("12")));
			Assert.That(utf8.FromInt(-12), Is.EqualTo(new utf8("-12")));
		}

		[Test]
		public void ParseLong()
		{
			Assert.That(utf8.ParseLong(new utf8("-1")), Is.EqualTo(-1));
			Assert.That(utf8.ParseLong(new utf8("1587")), Is.EqualTo(1587));
			Assert.That(utf8.ParseLong(new utf8("777777777777")), Is.EqualTo(777777777777L));
			Assert.That(utf8.ParseLong(new utf8("1")), Is.EqualTo(1L));
			Assert.That(utf8.ParseLong(new utf8("4")), Is.EqualTo(4L));
		}

		[Test]
		public void ParseLongRadix()
		{
			Assert.That(utf8.ParseLong(new utf8("-1"), 16), Is.EqualTo(-0x1));
			Assert.That(utf8.ParseLong(new utf8("1587"), 16), Is.EqualTo(0x1587));
			Assert.That(utf8.ParseLong(new utf8("777777777777"), 16), Is.EqualTo(0x777777777777L));
		}

		[Test]
		public void TrimSingleChar()
		{
			Console.WriteLine("trimmed: [{0}]", new utf8("4").TrimEnd());
			Assert.That(new utf8("4").Trim(), Is.EqualTo(new utf8("4")));
		}

		[Test]
		public void Split()
		{
			var str = new utf8("0000;<control>;Cc;0;BN;;;;;N;NULL;;;;");
			var list = str.Split(new char[] { ';' });
			Assert.That(list, Is.EqualTo(new utf8[] {
				new utf8("0000"),
				new utf8("<control>"),
				new utf8("Cc"),
				new utf8("0"),
				new utf8("BN"),
				utf8.Empty,
				utf8.Empty,
				utf8.Empty,
				utf8.Empty,
				new utf8("N"),
				new utf8("NULL"),
				utf8.Empty,
				utf8.Empty,
				utf8.Empty,
				utf8.Empty,
			}));
		}

		[Test]
		public void ToUpper()
		{
			Assert.That(new utf8("hello WORLD").ToUpper(), Is.EqualTo(new utf8("HELLO WORLD")));
		}

		[Test]
		public void ToUpperOldHungarian()
		{
			// System.String doesn't correctly handle this.
			// Let's show that we're better.
			Assert.That(new utf8("\uD803\uDCC1").ToUpper(), Is.EqualTo(new utf8("\uD803\uDC81")));
		}
	}
}
