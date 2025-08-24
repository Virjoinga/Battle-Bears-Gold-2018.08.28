using System.Collections;
using UnityEngine;
public class InputField : MonoBehaviour
{
    public TextMesh textMesh;

    private keyboardPC.TouchScreenKeyboard keyboard;

    public string placeholderString;

    public string actualString;

    public bool passwordField;

    public bool emailField;

    public static bool isGrabbingInput;

    public int maxLength = 30;

    public bool numericOnly;

    public InputField otherField;

    private static InputField current;

    public string Value
    {
        get
        {
            return actualString;
        }
        set
        {
            setText(value);
        }
    }

    private string hideText(string actualString)
    {
        string text = string.Empty;
        for (int i = 0; i < actualString.Length; i++)
        {
            text += "*";
        }
        return text;
    }

    public void setText(string t)
    {
        actualString = t;
        if (passwordField)
        {
            textMesh.text = hideText(actualString);
        }
        else
        {
            textMesh.text = t;
        }
        SendMessageUpwards("OnInputFieldChanged", this, SendMessageOptions.DontRequireReceiver);
    }

    private void OnGUIButtonClicked(GUIButton b)
    {
        if (b.name == base.name && keyboard == null && !isGrabbingInput)
        {
            StartCoroutine(grabInput());
        }
    }

    private IEnumerator grabInput()
    {
        // Open your custom PC keyboard
        keyboard = keyboardPC.TouchScreenKeyboard.Open(
            textMesh.text,
            emailField ? TouchScreenKeyboardType.EmailAddress : TouchScreenKeyboardType.Default,
            false, false, passwordField, false, placeholderString
        );

        GUIController guiController = base.transform.root.GetComponentInChildren<GUIController>();
        if (guiController != null) guiController.IsActive = false;

        isGrabbingInput = true;

        while (!keyboard.done && keyboard.active)
        {
            actualString = keyboard.text;
            if (actualString.Length > maxLength)
            {
                actualString = actualString.Substring(0, maxLength);
                keyboard.text = actualString;
            }

            textMesh.text = passwordField ? hideText(actualString) : actualString;
            yield return new WaitForSeconds(0.1f);
        }

        if (guiController != null) guiController.IsActive = true;
        isGrabbingInput = false;
        SendMessageUpwards("OnInputFieldChanged", this);
        keyboard = null;
    }

}
