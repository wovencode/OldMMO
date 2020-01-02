// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using OpenMMO.Groundwork;
using UnityEngine;
using UnityEngine.UI;

namespace OpenMMO.Groundwork {

	// ===================================================================================
	// OM_UI_PanelMessage
	// ===================================================================================
	public partial class OM_UI_PanelMessage : MonoBehaviour {
		
		[Header("---------- [Required] UI Elements ----------")]
	    public GameObject panelRoot;
	    public Image background;
	    public Text text;
	    public Image icon;
	    [Range(1,99)]public float displayDuration = 2f;
	    
	    [Header("---------- [Optional] Settings ----------")]
	    public Sprite[] icons;
	    public AudioClip[] sounds;
	    
	    protected AudioSource audioSource;
	    
		//--------------------------------------------------------------------------------
		// Show
		//--------------------------------------------------------------------------------
	    public void Show(string msg="", int icn=-1, int snd=-1) {
	    	
	    	if (msg != "")
	    	{
	    		if (panelRoot != null && text != null)
	    		{
	    			
	    			text.text = msg;
	    			
	    			if (!audioSource)
	    				audioSource = GetComponent<AudioSource>();
					
					if (icn > -1 && icons.Length >= icn) {
						icon.sprite = icons[icn];
						icon.gameObject.SetActive(true);
					}
					else
					{
						icon.gameObject.SetActive(false);
					}
					
					if (snd > -1 && sounds.Length >= snd && sounds[snd] != null)
						audioSource.PlayOneShot(sounds[snd]);
					
					FadeIn(displayDuration/4);
					Invoke("FadeOut", displayDuration/2);
	        		Invoke("Hide", displayDuration);
	        
	        	}
	        	else
	        	{
    				Debug.LogWarning(Constants.STR_ERROR_MISSING_UI + this.name);
    			}
    			
    		}
    		
	    }
	    
		// -----------------------------------------------------------------------------------
		// FadeIn
		// -----------------------------------------------------------------------------------
		protected void FadeIn(float duration) {
			background.GetComponent<CanvasRenderer>().SetAlpha(0.01f);
			text.GetComponent<CanvasRenderer>().SetAlpha(0.01f);
			icon.GetComponent<CanvasRenderer>().SetAlpha(0.01f);
			panelRoot.SetActive(true);
			background.CrossFadeAlpha(1, duration, true);
			text.CrossFadeAlpha(1, duration, true);
			icon.CrossFadeAlpha(1, duration, true);
			
		}
	
		// -----------------------------------------------------------------------------------
		// FadeOut
		// -----------------------------------------------------------------------------------
		protected void FadeOut() {
			background.CrossFadeAlpha(0, displayDuration/4, true);
			text.CrossFadeAlpha(0, displayDuration/4, true);
			icon.CrossFadeAlpha(0, displayDuration/4, true);
		}
	    
	    //--------------------------------------------------------------------------------
		// Hide
		//--------------------------------------------------------------------------------
	    public void Hide() {
	    	if (text != null)
	    		text.text = "";
	    	panelRoot.SetActive(false);
	    }
	    
	    //--------------------------------------------------------------------------------
	    
	}

}

// =======================================================================================