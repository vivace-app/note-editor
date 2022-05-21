using System;
using NoteEditor.Notes;
using NoteEditor.Model;
using NoteEditor.Utility;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace NoteEditor.Presenter
{
    public class ToggleEditTypePresenter : MonoBehaviour
    {
        [SerializeField] Button longNoteToggleButton = default;
        [SerializeField] Button flickNoteToggleButton = default;
        [SerializeField] Sprite iconLongNotes = default;
        [SerializeField] Sprite iconSingleNotes = default;
        [SerializeField] Sprite iconArrowDefault = default;
        [SerializeField] Sprite iconArrowLeftward = default;
        [SerializeField] Sprite iconArrowRightward = default;
        [SerializeField] Sprite iconArrowUpward = default;
        [SerializeField] Color longTypeStateButtonColor = default;
        [SerializeField] Color singleTypeStateButtonColor = default;

        void Awake()
        {
            // Long Note Mode
            if (longNoteToggleButton != null)
            {
                longNoteToggleButton.OnClickAsObservable()
                    .Merge(this.UpdateAsObservable().Where(_ => KeyInput.AltKeyDown()))
                    .Select(_ => EditState.NoteType.Value != NoteTypes.Long ? NoteTypes.Long : NoteTypes.Single)
                    .Subscribe(editType => EditState.NoteType.Value = editType);

                var longNoteButtonImage = longNoteToggleButton.GetComponent<Image>();

                EditState.NoteType.Select(_ => EditState.NoteType.Value == NoteTypes.Long)
                    .Subscribe(isLongType =>
                    {
                        longNoteButtonImage.sprite = isLongType ? iconLongNotes : iconSingleNotes;
                        longNoteButtonImage.color = isLongType ? longTypeStateButtonColor : singleTypeStateButtonColor;
                    });
            }
            
            
            // Flick Note Mode
            if (flickNoteToggleButton != null)
            {
                flickNoteToggleButton.OnClickAsObservable()
                    .Merge(this.UpdateAsObservable().Where(_ => KeyInput.TabKeyDown()))
                    .Select(_ =>
                        EditState.NoteType.Value switch
                        {
                            NoteTypes.Single => NoteTypes.UpwardFlick,
                            NoteTypes.Long => NoteTypes.UpwardFlick,
                            NoteTypes.LeftwardFlick => NoteTypes.Single,
                            NoteTypes.RightwardFlick => NoteTypes.LeftwardFlick,
                            NoteTypes.UpwardFlick => NoteTypes.RightwardFlick,
                            _ => throw new ArgumentOutOfRangeException()
                        })
                    .Subscribe(editType => EditState.NoteType.Value = editType);

                var flickNoteButtonImage = flickNoteToggleButton.GetComponent<Image>();

                EditState.NoteType.Select(_ => EditState.NoteType.Value)
                    .Subscribe(noteType =>
                    {
                        switch (noteType)
                        {
                            case NoteTypes.Single:
                                flickNoteButtonImage.sprite = iconArrowDefault;
                                flickNoteButtonImage.color = singleTypeStateButtonColor;
                                break;
                            case NoteTypes.Long:
                                flickNoteButtonImage.sprite = iconArrowDefault;
                                flickNoteButtonImage.color = singleTypeStateButtonColor;
                                break;
                            case NoteTypes.LeftwardFlick:
                                flickNoteButtonImage.sprite = iconArrowLeftward;
                                flickNoteButtonImage.color = longTypeStateButtonColor;
                                break;
                            case NoteTypes.RightwardFlick:
                                flickNoteButtonImage.sprite = iconArrowRightward;
                                flickNoteButtonImage.color = longTypeStateButtonColor;
                                break;
                            case NoteTypes.UpwardFlick:
                                flickNoteButtonImage.sprite = iconArrowUpward;
                                flickNoteButtonImage.color = longTypeStateButtonColor;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(noteType), noteType, null);
                        }
                    });
            }
        }
    }
}