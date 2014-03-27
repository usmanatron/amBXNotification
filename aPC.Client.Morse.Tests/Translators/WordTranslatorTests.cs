﻿using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using aPC.Client.Morse.Translators;
using aPC.Client.Morse.Codes;

namespace aPC.Client.Morse.Tests.Translators
{
  [TestFixture]
  class WordTranslatorTests
  {
    [Test]
    public void InputWord_CannotContainSpaces()
    {
      Assert.Throws<InvalidOperationException>(() => new WordTranslator("Space Space"));
    }

    [Test]
    public void SingleCharacterWord_ReturnsTheCharacter()
    {
      var lCharacter = 'A';

      var lTranslatedWord = new WordTranslator(lCharacter.ToString()).Translate();
      var lTranslatedCharacter = new CharacterTranslator(lCharacter).Translate();

      Assert.AreEqual(lTranslatedCharacter.Count(), lTranslatedWord.Count());
      for (var i = 0; i < lTranslatedCharacter.Count(); i++)
      {
        Assert.AreEqual(lTranslatedCharacter[i].GetType(), lTranslatedWord[i].GetType());
      }
    }

    [Test]
    public void TwoCharacters_ReturnsCharactersWithOneSeparator()
    {
      var lFirstCharacter = '-';
      var lSecondCharacter = '?';

      var lTranslatedWord = new WordTranslator(lFirstCharacter.ToString() + lSecondCharacter.ToString()).Translate();
      
      var lFirstTranslatedCharacter = new CharacterTranslator(lFirstCharacter).Translate();
      var lSecondTranslatedCharacter = new CharacterTranslator(lSecondCharacter).Translate();
      var lExpectedWord = lFirstTranslatedCharacter;
      lExpectedWord.Add(new CharacterSeparator());
      lExpectedWord.AddRange(lSecondTranslatedCharacter);

      Assert.AreEqual(lExpectedWord.Count(), lTranslatedWord.Count());
      for (var i = 0; i < lExpectedWord.Count(); i++)
      {
        Assert.AreEqual(lExpectedWord[i].GetType(), lTranslatedWord[i].GetType());
      }
    }

    [Test]
    [TestCaseSource("TestWords")]
    public void SomeExampleWords_ReturnExpectedMorseCode(TestWordData xiData)
    {
      var lTranslatedWord = new WordTranslator(xiData.Word).Translate();

      Assert.AreEqual(xiData.ExpectedCodeCount, lTranslatedWord.Count);

      for (int i = 0; i < xiData.ExpectedCodeCount; i++)
      {
        Assert.AreEqual(xiData.ExpectedCode[i].GetType(), lTranslatedWord[i].GetType());
      }
    }

    private TestWordData[] TestWords = new TestWordData[]
    {
      new TestWordData("CAR6", new List<IMorseBlock> 
      {
        new Dash(), new DotDashSeparator(), new Dot(), new DotDashSeparator(), new Dash(), new DotDashSeparator(), new Dot(), new CharacterSeparator(), 
        new Dot(), new DotDashSeparator(), new Dash(), new CharacterSeparator(), 
        new Dot(), new DotDashSeparator(), new Dash(), new DotDashSeparator(), new Dot(), new CharacterSeparator(), 
        new Dash(), new DotDashSeparator(), new Dot(), new DotDashSeparator(), new Dot(), new DotDashSeparator(), new Dot(), new DotDashSeparator(), new Dot() 
      }),
      new TestWordData("BLOB", new List<IMorseBlock> 
      {
        new Dash(), new DotDashSeparator(), new Dot(), new DotDashSeparator(), new Dot(), new DotDashSeparator(), new Dot(), new CharacterSeparator(),
        new Dot(), new DotDashSeparator(), new Dash(), new DotDashSeparator(), new Dot(), new DotDashSeparator(), new Dot(), new CharacterSeparator(),
        new Dash(), new DotDashSeparator(), new Dash(), new DotDashSeparator(), new Dash(), new CharacterSeparator(),
        new Dash(), new DotDashSeparator(), new Dot(), new DotDashSeparator(), new Dot(), new DotDashSeparator(), new Dot()
      }),
      new TestWordData("WHY?", new List<IMorseBlock> 
      {
        new Dot(), new DotDashSeparator(), new Dash(), new DotDashSeparator(), new Dash(), new CharacterSeparator(),
        new Dot(), new DotDashSeparator(), new Dot(), new DotDashSeparator(), new Dot(), new DotDashSeparator(), new Dot(), new CharacterSeparator(),
        new Dash(), new DotDashSeparator(), new Dot(), new DotDashSeparator(), new Dash(), new DotDashSeparator(), new Dash(), new CharacterSeparator(),
        new Dot(), new DotDashSeparator(), new Dot(), new DotDashSeparator(), new Dash(), new DotDashSeparator(), new Dash(), new DotDashSeparator(), new Dot(), new DotDashSeparator(), new Dot()
      })
    };
  }
}