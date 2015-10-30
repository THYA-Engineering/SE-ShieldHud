/* V1.01 
 
Welcome to THYA's Shield HUD Script. Please follow this setup procedure. 
 
Setup: 
	1.Choose The LCD Mode: 
         0 - Text Only 
         1 - Bars and Text 
         2 - Horizontal Images 
         3 - Vertical Images 
*/
const int MODE = 2; 
/*
	2.If you're using Text, choose the Text LCD Size: 
         SMALL - For Small LCDs 
         LARGE - For Large LCDs 
*/
const string LCD_SIZE = "LARGE"; 
/*
	3.Edit the Text Bar options as you wish. 
*/
const string L_BAR_SURROUND = "{"; // Left bar surround 
const string R_BAR_SURROUND = "}"; // Right bar surround 
const string BAR_FILLER = "[]"; // Used to fill the bars with text 
const string BAR_BLANK_FILLER = ".."; // Blank Filler for bar 
/*
	4.Edit the Text Bar Colors as you wish.
*/
Color COLOR_HIGH = Color.Green;
Color COLOR_MEDIUM = Color.Yellow; 
Color COLOR_LOW = Color.Red; 
/*	
	5.Edit the Text font size. 
*/
const float LARGE_FONT_SIZE = 3.0f; 
const float SMALL_FONT_SIZE = 2.3f; 
/*
	6.Edit your LCD's name to match the name in LCD_Name. 
*/
const string LCD_NAME = "Shield LCD"; 
/*
	7.Build a timer block with the time to 1 second and click "Setup Actions". 
	8.Action #1 should be "run" the program block but leave the argument field blank. 
	9.Action #2 should be "start" the timer block. 
	10. If you're using LCD Graphics, please install our graphics pack if you have not already: http://bit.ly/1PVsZQ6 
	  
	  
	Bars and Text Output Example: 
        Shields: 40% 
        {[][][][][]...........}	 
		 
		 
	Troubleshooting: 
	If you have an exception you will have to fix the issue "remember and exit" and then "run" to refresh the system. 
	 
*/ 
 
// Settings 

const int SHIELD_HIGH = 80; // High shield value  
const int SHIELD_LOW = 30; // Low shield value 
 
 
/* DO NOT EDIT BELOW THIS LINE */ 
 
void Main(string argument) { 
    IMyTextPanel lcdDisplay = null; 
    List<IMyTerminalBlock> shieldGenerator = new List<IMyTerminalBlock>(); 
 
    lcdDisplay = initLCD(LCD_NAME); 
 
    GridTerminalSystem.SearchBlocksOfName("Shield:", shieldGenerator); 
    if (shieldGenerator.Count == 0) { 
        throw new Exception("Could not find any shield generators!"); 
    }    else if (!(shieldGenerator[0].CustomName.Contains("(") || shieldGenerator[0].CustomName.Contains(")"))) { 
        throw new Exception("Block named is not a shield generator"); 
    } 
 
    string shield_name = new String(shieldGenerator[0].CustomName.ToCharArray()); 
 
    int [] values = split_string(shield_name); 
    int current_shield = values[0]; 
    int max_shield = values[1]; 
 
    int percent = calcPercent(current_shield, max_shield); 
 
    string image_prefix = ""; 
    string text = ""; 
 
    switch(MODE) { 
        case 0: 
        // Text Only 
            if (LCD_SIZE == "LARGE") { 
                prepLCD(lcdDisplay, percent, LARGE_FONT_SIZE); 
                text = textFactory(current_shield, max_shield, percent); 
            } else if (LCD_SIZE == "SMALL") { 
                prepLCD(lcdDisplay, percent, SMALL_FONT_SIZE); 
                text = textFactory(current_shield, max_shield, percent); 
            } else { 
                text = "Incorrect Bar size.\nChoose either\nLARGE or SMALL"; 
            } 
            setTextLCD(text, lcdDisplay); 
            break; 
        case 1: 
        // Bars and Text 
        if (LCD_SIZE == "LARGE") { 
            prepLCD(lcdDisplay, percent, LARGE_FONT_SIZE); 
            text = largeBarFactory(current_shield, max_shield, percent); 
        }        else if (LCD_SIZE == "SMALL") { 
            prepLCD(lcdDisplay, percent, SMALL_FONT_SIZE); 
            text = smallBarFactory(current_shield, max_shield, percent); 
        }        else { 
            text = "Incorrect Bar size.\nChoose either\nLARGE or SMALL"; 
        } 
 
        setTextLCD(text, lcdDisplay); 
        break; 
        case 2: 
        // Horizontal Images 
            image_prefix = "THYA-ShieldH"; 
        text = imageNameFactory(current_shield, max_shield, percent, image_prefix); 
        setImage(text, lcdDisplay); 
        break; 
        case 3: 
        // Vertical Images 
            image_prefix = "THYA-ShieldV"; 
        text = imageNameFactory(current_shield, max_shield, percent, image_prefix); 
        setImage(text, lcdDisplay); 
        break; 
        case 4: 
        // Circular Images 
            image_prefix = "THYA-ShieldC"; 
        text = imageNameFactory(current_shield, max_shield, percent, image_prefix); 
        setImage(text, lcdDisplay); 
        break; 
        default: 
        // Throw error 
            throw new Exception("Incorrect mode: " + MODE.ToString() + "\nPlease choose a valid mode"); 
        break; 
    } 
} 
 
int calcPercent(int current_shield, int max_shield) { 
    if (max_shield == 0) { 
        return 0; 
    } 
 
    return (current_shield * 100) / max_shield; 
} 
 
string textFactory(int current_shield, int max_shield, int percent) { 
    string data = "Shields:" + percent.ToString() + "%\n Current:" + current_shield.ToString() 
    + "\n      Max:" + max_shield.ToString(); 
    return data; 
} 
 
int [] split_string(string shield_name) { 
 
    string [] tempStringArray = null; 
    string tempString = null; 
    string [] splitString = null; 
 
    char [] colonSplit = { ':' }; 
    char [] slashSplit = { '/' }; 
 
    tempStringArray = shield_name.Split(colonSplit); 
 
    tempString = tempStringArray[1]; 
    tempString = tempString.Replace("(", ""); 
    tempString = tempString.Replace(")", ""); 
    tempString = tempString.Trim(); 
 
    splitString = tempString.Split(slashSplit); 
 
    int [] values = { Int32.Parse(splitString[0]), Int32.Parse(splitString[1]) }; 
    return values; 
} 
 
void prepLCD(IMyTextPanel lcdPanel, int percent, float font) { 
    lcdPanel.SetValueFloat("FontSize", font); 
 
    if (percent > SHIELD_HIGH) { 
        lcdPanel.SetValue("FontColor", COLOR_HIGH); 
    }    else if (percent <= SHIELD_HIGH && percent > SHIELD_LOW) { 
        lcdPanel.SetValue("FontColor", COLOR_MEDIUM); 
    }    else { 
        lcdPanel.SetValue("FontColor", COLOR_LOW); 
    } 
} 
 
string smallBarFactory(int current_shield, int max_shield, int percent) { 
    string bar = "Shields: " + percent.ToString() + "%\n" + L_BAR_SURROUND; 
     
    percent = percent / 5; 
    int i = 0; 
    for (;i < 10; i++) { 
        if (i < percent) { 
            bar += BAR_FILLER; 
        } else { 
            bar += BAR_BLANK_FILLER; 
        } 
    } 
 
    bar += R_BAR_SURROUND + "\n" + L_BAR_SURROUND; 
 
    for (;i < 20; i++) { 
        if (i < percent) { 
            bar += BAR_FILLER; 
        } else { 
            bar += BAR_BLANK_FILLER; 
        } 
    } 
 
    bar += R_BAR_SURROUND + "\n\nCurrent:" + current_shield.ToString("N0") 
    + "\nMaximum:" + max_shield.ToString("N0"); 
    return bar; 
} 
 
string largeBarFactory(int current_shield, int max_shield, int percent) { 
    string bar = "Shields: " + percent.ToString() + "%\n" + L_BAR_SURROUND; 
     
    percent = percent / 5; 
    for (int i = 0; i < 20; i++) { 
        if (i < percent) { 
            bar += BAR_FILLER; 
        }		else { 
            bar += BAR_BLANK_FILLER; 
        } 
    } 
 
    bar += R_BAR_SURROUND + "\n\nCurrent:" + current_shield.ToString("N0") + "\nMaximum:" + max_shield.ToString("N0"); 
 
    return bar; 
} 
 
string imageNameFactory(int current_shield, int max_shield, int percent, string prefix) { 
    string image_name = ""; 
 
    if(percent == 0) return prefix + "000"; 
 
    if (percent < 10) { 
        image_name = prefix + "00" + percent.ToString(); 
    }	else if (percent < 100) { 
        image_name = prefix + "0" + percent.ToString(); 
    }	else if (percent == 100) { 
        image_name = prefix + percent.ToString(); 
    } 
 
    return image_name; 
} 
 
IMyTextPanel initLCD(string lcd_name) { 
    IMyTextPanel lcd = GridTerminalSystem.GetBlockWithName(lcd_name) as IMyTextPanel; 
    if (lcd == null) { 
        throw new Exception("LCD Panel Not Found! " + lcd_name); 
    } 
    return lcd; 
} 
 
void setImage(String imageName, IMyTextPanel lcd) { 
    lcd.ShowTextureOnScreen(); 
    lcd.ClearImagesFromSelection(); 
    lcd.AddImageToSelection(imageName); 
} 
 
void setTextLCD(string text, IMyTextPanel lcd) { 
    lcd.WritePublicText(text); 
    lcd.ShowPublicTextOnScreen(); 
}