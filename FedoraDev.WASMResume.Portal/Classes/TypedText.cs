using System;
using System.Linq;
using System.Threading.Tasks;

namespace FedoraDev.WASMResume.Portal.Classes
{
	public class TypedText
	{
		public Action<string> TextUpdated { get; set; }

		string TextContent => $"I {_text}{Cursor}";
		string Cursor => _showCursor ? "|" : "&nbsp;";

		readonly string[] _possibleTexts;
		readonly float _characterTypeDelay;
		readonly float _characterTypeVariance;
		readonly float _repeatDelay;
		readonly float _cursorBlinkDelay;
		readonly Random _random = new();

		int[] _textOrder;
		int _currentTextIndex = 0;
		string _text;
		string _nextText;
		bool _showCursor = true;

		public TypedText(string[] possibleTexts, float charactersPerSecond = 5f, float typeSpeedVariance = 0.02f, float repeatDelay = 5f, float cursorBlinksPerSecond = 1f)
		{
			_possibleTexts = possibleTexts;
			_text = _possibleTexts[0];
			_nextText = _possibleTexts[0];

			_textOrder = new int[_possibleTexts.Length];
			for (int i = 0; i < _possibleTexts.Length; i++)
				_textOrder[i] = i;

			_characterTypeDelay = charactersPerSecond / 60f;
			_characterTypeVariance = typeSpeedVariance;
			_repeatDelay = repeatDelay;
			_cursorBlinkDelay = 1f / (cursorBlinksPerSecond * 2f);
		}

		public async Task InitializeAsync()
		{
			TextUpdated?.Invoke(TextContent);

			_ = BlinkCursor();

			_textOrder = new int[_possibleTexts.Length];
			for (int i = 0; i < _possibleTexts.Length; i++)
				_textOrder[i] = i;

			while (true)
			{
				await Task.Delay((int)(_repeatDelay * 1000));
				await DeleteCurrentTitle();
				ChooseNextTitle();
				await WriteNextTitle();
			}
		}

		async Task BlinkCursor()
		{
			while (true)
			{
				await Task.Delay((int)(_cursorBlinkDelay * 1000));
				_showCursor = !_showCursor;
				TextUpdated?.Invoke(TextContent);
			}
		}

		async Task DeleteCurrentTitle()
		{
			while (_text.Length > 0)
			{
				await Task.Delay(GetCharacterTypeDelay());
				_text = _text[0..^1];
				TextUpdated?.Invoke(TextContent);
			}
		}

		void ChooseNextTitle()
		{
			_currentTextIndex++;
			if (_currentTextIndex == _possibleTexts.Length)
			{
				_currentTextIndex = 0;
				_textOrder = _textOrder.OrderBy(x => _random.Next()).ToArray();
			}

			_nextText = _possibleTexts[_textOrder[_currentTextIndex]];
		}

		async Task WriteNextTitle()
		{
			while (_text.Length < _nextText.Length)
			{
				await Task.Delay(GetCharacterTypeDelay());
				_text += _nextText.Substring(_text.Length, 1);
				TextUpdated?.Invoke(TextContent);
			}
		}

		int GetCharacterTypeDelay()
		{
			if (_characterTypeVariance == 0)
				return (int)(_characterTypeDelay * 1000);

			int delayMin = (int)((_characterTypeDelay - _characterTypeVariance) * 1000);
			int delayMax = (int)((_characterTypeDelay + _characterTypeVariance) * 1000);
			return _random.Next(delayMin, delayMax);
		}
	}
}
