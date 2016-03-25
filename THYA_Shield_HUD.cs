/* V1.3.2

Welcome to THYA's Shield HUD Script.

Here is the Script's Github link in case you want to check it out!
https://github.com/THYA-Engineering/SE-ShieldHud

Setup Instructions:
Follow these steps changing only the values after = some of which are in "Quotes" if need be.
------------------------------------------------------------------
1. Install Beta Energy Shields by Cython from the Steam and add it to your mod list.
   Workshop: http://steamcommunity.com/workshop/filedetails/?id=484504816
------------------------------------------------------------------
2. Install the THYA Shield HUD Graphics Pack and add it to your mod list: http://bit.ly/1PVsZQ6
------------------------------------------------------------------
3. Name just one of your Shield Generators "Shield:"
------------------------------------------------------------------
4. Edit your LCD's name to match the name of LCD_Name.
*/
const string LCD_NAME = "Shield LCD";
/*
------------------------------------------------------------------
5. Choose Your Shield LCD Mode:
         0 - Text Display Only
         1 - Bars and Text Shield Display
         2 - Horizontal Shield Bar
         3 - Vertical Shield Bar
         4 - Arched Shield Bar
*/
const int MODE = 1;
/*
------------------------------------------------------------------
6. Build a timer block and set the delay to 1 second.
------------------------------------------------------------------
7.  Click "Setup Actions" and set Action #1 to "run" the program block with the argument field blank.
------------------------------------------------------------------
8. Set Action #2 to "start" the timer block.
------------------------------------------------------------------
9. If you're using Text or Text Bars "LCD MODES 0 or 1" choose the LCD Size:
         SMALL - For Small LCDs
         LARGE - For Large LCDs
*/
const string LCD_SIZE = "SMALL";
/*
------------------------------------------------------------------
10. Edit the Text Bar options if you wish.
*/
const string L_BAR_SURROUND = " {"; // Left bar surround
const string R_BAR_SURROUND = "}"; // Right bar surround
const string BAR_FILLER = "[]"; // Used to fill the bars with text
const string BAR_BLANK_FILLER = ".."; // Blank Filler for bar
/*
------------------------------------------------------------------
11. Edit the Text Bar Colors if you wish.
*/
Color COLOR_HIGH = Color.Green;
Color COLOR_MEDIUM = Color.Yellow;
Color COLOR_LOW = Color.Orange;
Color COLOR_CRIT = Color.Red;
/*
------------------------------------------------------------------
12. Edit the Text font size if you wish.
*/
const float LARGE_FONT_SIZE = 3.0f;
const float SMALL_FONT_SIZE = 2.5f;
/*
------------------------------------------------------------------
13. These values are the % when the shield text changes colors. Edit them if you wish.
*/
const int SHIELD_HIGH = 74; // High shield value
const int SHIELD_LOW = 49; // Low shield value
const int SHIELD_CRIT = 24; // Critical shield value
/*
------------------------------------------------------------------

Troubleshooting:
-If you have an exception you need to fix the issue then "remember and exit" and "run" to refresh the system.
-Timers seem to stop working after power loss so make sure your timer has started.
-Don't forget to add start timer to the end of your timer actions to create a loop.
-Shield generators bug out sometimes so you may just need to reload the game.
-You shield generator must be named Shield: DO NOT FORGET THE : a lot of people seem to.
-It is possible you accidentally bugged the code so try deleting the code and reloading it off the workshop.

/* -----DO NOT EDIT BELOW THIS LINE----- */

void Main(string argument) {
    if (string.IsNullOrEmpty(Storage)) {
        Storage = "0:0";
    }
    List<IMyTerminalBlock> lcdDisplays = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> shieldGenerator = new List<IMyTerminalBlock>();

    lcdDisplays = initLCD(LCD_NAME);

    GridTerminalSystem.SearchBlocksOfName("Shield:", shieldGenerator);
    if (shieldGenerator.Count == 0) {
        setTextLCD("NO\nSHIELD\nGENERATORS\nFOUND", lcdDisplays);
        throw new Exception("Could not find any shield generators!");
    } else if (!(shieldGenerator[0].CustomName.Contains("(") || shieldGenerator[0].CustomName.Contains(")"))) {
        setTextLCD("BLOCK NAMED IS NOT A SHIELD GENERATOR!", lcdDisplays);
        throw new Exception("Block named is not a shield generator");
    }

    string shield_name = new String(shieldGenerator[0].CustomName.ToCharArray());

    int [] values = split_string(shield_name);
    int current_shield = values[0];
    int max_shield = values[1];
    int sps = calcSPS(current_shield, max_shield);

    int percent = calcPercent(current_shield, max_shield);

    string image_prefix = "";
    string text = "";

    switch(MODE) {
        case 0:
        // Text Only
            if (LCD_SIZE == "LARGE") {
                prepLCD(lcdDisplays, percent, LARGE_FONT_SIZE);
                text = textFactory(current_shield, max_shield, percent, sps);
            } else if (LCD_SIZE == "SMALL") {
                prepLCD(lcdDisplays, percent, SMALL_FONT_SIZE);
                text = textFactory(current_shield, max_shield, percent, sps);
            } else {
                text = "Incorrect Bar size.\nChoose either\nLARGE or SMALL";
            }
            setTextLCD(text, lcdDisplays);
            break;
        case 1:
            // Bars and Text
            if (LCD_SIZE == "LARGE") {
                prepLCD(lcdDisplays, percent, LARGE_FONT_SIZE);
                text = largeBarFactory(current_shield, max_shield, percent, sps);
            }        else if (LCD_SIZE == "SMALL") {
                prepLCD(lcdDisplays, percent, SMALL_FONT_SIZE);
                text = smallBarFactory(current_shield, max_shield, percent, sps);
            }        else {
                text = "Incorrect Bar size.\nChoose either\nLARGE or SMALL";
            }

            setTextLCD(text, lcdDisplays );
            break;
        case 2:
        // Horizontal Images
            image_prefix = "THYA-ShieldH";
            text = imageNameFactory(current_shield, max_shield, percent, image_prefix);
            setImage(text, lcdDisplays );
            break;
        case 3:
        // Vertical Images
            image_prefix = "THYA-ShieldV";
            text = imageNameFactory(current_shield, max_shield, percent, image_prefix);
            setImage(text, lcdDisplays );
            break;
        case 4:
        // Circular Images
            image_prefix = "THYA-ShieldC";
            text = imageNameFactory(current_shield, max_shield, percent, image_prefix);
            setImage(text, lcdDisplays );
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
    double dCurrent_shield = current_shield;
    double dMax_shield = max_shield;
    double dPercent = dCurrent_shield / dMax_shield;

    return Convert.ToInt32(dPercent * 100);
}

string textFactory(int current_shield, int max_shield, int percent, int sps) {
    string data = "    Shields:" + percent.ToString() + "%\n\n Shields:" + formatNum(current_shield)
    + "\n       Full:" + formatNum(max_shield) + "\n       S\\s:" + sps.ToString();
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

void prepLCD(List<IMyTerminalBlock> lcdPanels, int percent, float font) {
    for(var i = 0; i < lcdPanels.Count; i++) {
        IMyTextPanel lcdPanel = (IMyTextPanel)lcdPanels[i];

        lcdPanel.SetValueFloat("FontSize", font);

        if (percent > SHIELD_HIGH) {
            lcdPanel.SetValue("FontColor", COLOR_HIGH);
        } else if (percent <= SHIELD_HIGH && percent > SHIELD_LOW) {
            lcdPanel.SetValue("FontColor", COLOR_MEDIUM);
        } else if (percent <= SHIELD_LOW && percent > SHIELD_CRIT) {
            lcdPanel.SetValue("FontColor", COLOR_LOW);
        } else {
            lcdPanel.SetValue("FontColor", COLOR_CRIT);
        }
    }
}

string smallBarFactory(int current_shield, int max_shield, int percent, int sps) {
    string bar = " Shields: " + percent.ToString() + "%\n" + L_BAR_SURROUND;

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

    bar += R_BAR_SURROUND + "\n\n Shields:" + formatNum(current_shield)
+ "\n Full:" + formatNum(max_shield) + "\n S\\s:" + sps.ToString();
    return bar;
}

string largeBarFactory(int current_shield, int max_shield, int percent, int sps) {
    string bar = "            Shields: " + percent.ToString() + "%\n" + L_BAR_SURROUND;

    percent = percent / 5;
    for (int i = 0; i < 20; i++) {
        if (i < percent) {
            bar += BAR_FILLER;
        }       else {
            bar += BAR_BLANK_FILLER;
        }
    }

    bar += R_BAR_SURROUND + "\n          Shields:" + formatNum(current_shield) + "\n                Full:" + formatNum(max_shield) + "\n                S\\s:" + sps.ToString();

    return bar;
}

string imageNameFactory(int current_shield, int max_shield, int percent, string prefix) {
    string image_name = "";

    if(percent == 0) return prefix + "000";

    if (percent < 10) {
        image_name = prefix + "00" + percent.ToString();
    }   else if (percent < 100) {
        image_name = prefix + "0" + percent.ToString();
    }   else if (percent == 100) {
        image_name = prefix + percent.ToString();
    }

    return image_name;
}

List<IMyTerminalBlock> initLCD(string lcd_name) {
    List<IMyTerminalBlock> lcds = new List<IMyTerminalBlock>();
    GridTerminalSystem.SearchBlocksOfName(lcd_name, lcds);

    if (lcds.Count == 0) {
        throw new Exception("Could not find any LCDs!");
    }

    return lcds;
}

void setImage(String imageName, List<IMyTerminalBlock> lcds) {
    for(var i = 0; i < lcds.Count; i++) {
        IMyTextPanel lcd = (IMyTextPanel)lcds[i];

        lcd.ShowTextureOnScreen();
        lcd.ClearImagesFromSelection();
        lcd.AddImageToSelection(imageName);
    }
}

void setTextLCD(string text, List<IMyTerminalBlock> lcds) {
    for(var i = 0; i < lcds.Count; i++) {
        IMyTextPanel lcd = (IMyTextPanel)lcds[i];

        lcd.WritePublicText(text);
        lcd.ShowPublicTextOnScreen();
    }
}

int calcSPS(int current_shield, int max_shield) {
    int [] values = getData();

    int sps = current_shield - values[0];
    if(current_shield == max_shield) {
        sps = 0;
        storeData(current_shield, sps);
        return sps;
    }
    if(sps == 0) {
        storeData(current_shield, values[1]);
        return values[1];
    }
    storeData(current_shield, sps);
    return sps;
}

void storeData(int current_shield, int sps) {
    Storage = current_shield.ToString() + ":" + sps.ToString();
}

int [] getData() {
    char [] colonSplit = { ':' };
    string [] sData  = Storage.Split(colonSplit);
    int [] iData = {Convert.ToInt32(sData[0]), Convert.ToInt32(sData[1])};
    return iData;
}

string formatNum(int num) {
    if (num >= 1000000000) {
        return (num / 1000000000D).ToString("0.##b");
    } else if (num >= 1000000) {
        return (num / 1000000D).ToString("0.##m");
    } else if (num >= 100000) {
        return (num / 1000D).ToString("0.#k");
    } else if (num >= 1000) {
        return (num / 1000D).ToString("0.##k");
    } else {
        return num.ToString("N0");
    }
}
