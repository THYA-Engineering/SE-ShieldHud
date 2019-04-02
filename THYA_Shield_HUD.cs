/* V1.6.3

Welcome to THYA's Shield HUD Script.

Setup Instructions:
Follow these steps changing only the values after = some of which are in "Quotes" if need be.
----------------------------------------------------------------------------------------------------------------------------------
1. Install Energy Shields by Cython from Steam and add it to your mod list.
    Workshop: http://steamcommunity.com/workshop/filedetails/?id=484504816
      AND / OR
    Install Defense Shield Mod Pack by DarkStar from Steam and add it to your mod list.
    Workshop: http://steamcommunity.com/sharedfiles/filedetails/?id=1365616918
+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    Both Mods may be installed at the same time, and will coexist peacefully. (Thank you, JTurp)
+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
2. Install the THYA Shield HUD Graphics Pack and add it to your mod list: http://bit.ly/1PVsZQ6
    Workshop: http://steamcommunity.com/sharedfiles/filedetails/?id=540003236
----------------------------------------------------------------------------------------------------------------------------------
3. Name your Shield Generators WHATEVER YOU WANT! The script will find them automatically!
----------------------------------------------------------------------------------------------------------------------------------
4. LCD displays can be set up per screen now, rather than one type grid-wide.
    The 2 parts of this naming setup are below.
----------------------------------------------------------------------------------------------------------------------------------
4a. Edit the first half your LCD's name to include the text of lcdNamePrefix. */
const string lcdNamePrefix = "[Shield LCD"; /*
4b. Edit the second half of your LCD's name to one of the lcdMode strings to set your Shield LCD's display mode: */
const string lcdModeTDS = ":TDS]"; // Text Display Only - Small Font
const string lcdModeTDL = ":TDL]"; // Text Display Only - Large Font
const string lcdModeBTS = ":BTS]"; // Bars and Text Shield Display - Small Font
const string lcdModeBTL = ":BTL]"; // Bars and Text Shield Display - Large Font
const string lcdModeCTD = ":CTD]"; // Corner LCD Text Display, % on large grid - % and SPS on small grid
const string lcdModeCRB = ":CRB]"; // Corner LCD Rainbow Bar - See note below on CRB+CCB Mode to change text overlay
const string lcdModeCCB = ":CCB]"; // Corner LCD Colored Bar - See note below on CRB+CCB Mode to change text overlay
const string lcdModeGfxH = ":THYA-H]"; // Horizontal Shield Bar
const string lcdModeGfxV = ":THYA-V]"; // Vertical Shield Bar
const string lcdModeGfxC = ":THYA-C]"; // Curved Shield Bar
/*
+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    Note on the CRB + CCB Modes - I felt the naming box was getting crowded, so text overlays are changed
    	in the "Custom Data" of the LCD block.  These auto-size to the "pixel" size of all the corner LCDs.
    	• Blank, or any random text not covered below - gets horizontal "Shield" overlay.
    	• SR gets rotated "Shield" overlay (for vertical screen installation).
    	• ES gets "Energy Shield" overlay (ESR gets rotated "E Shield") - only displays energy shield charge.
    	• DS gets "Defense Shield" overlay (DSR gets rotated "D Shield") - only displays defense shield charge.
    	• SGDUAL gets both displayed on small grids (SGDUALR gets rotated) - not enough room for this on large grid.
           *SGDUAL does not mix display types - all rainbow or all colored, not one of each.
+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    So, to double clarify: if you want a screen to show Text Display Only - Large Font, the
    full name should read "[Shield LCD:TDL]".  Curved Shield Bar would be "[Shield LCD:THYA-C]".
    The script searches only for the parts of the name, so if you want to do "[Shield LCD 36:TDL]",
    or ":THYA-H][Shield LCD Engineering", you shouldn't have any problems.
+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
5. Edit the Text Bar options if you wish. */
const string lBarSurr = " {"; // Left bar surround
const string rBarSurr = "}"; // Right bar surround
const string BarEmpty = ".."; // Blank Filler for bar
const string BarFill = "[]"; // Used to fill the bars with text
/*--------------------------------------------------------------------------------------------------------------------------------
6. Edit the Text Bar Colors if you wish. */
Color txtColHigh = Color.Green;
Color txtColMed = Color.Yellow;
Color txtColLow = Color.Orange;
Color txtColCrit = Color.Red;
/*--------------------------------------------------------------------------------------------------------------------------------
7. Edit the Text font size if you wish. */
const float lFontSize = 3.0f; // Large
const float sFontSize = 2.5f; // Small
const float cFontSize = 2.2f; // Corner
/*--------------------------------------------------------------------------------------------------------------------------------
8. These values are the % when the shield text changes colors. Edit them if you wish. */
const int shieldHigh = 74; // High shield value
const int shieldLow = 49; // Low shield value
const int shieldCrit = 24; // Critical shield value
/*--------------------------------------------------------------------------------------------------------------------------------
9. These values are the names for warning lights or sound blocks, and
the percentages at which the warning lights or sound blocks activate. */
const string shieldAudioName = "[Shield Alarm]";
const int shieldAudioWarning = 24; // Sound Block %
const string shieldLightName = "[Low Shield Light]";
const int shieldVisualWarning = 24; // Light %
/*
This is a pretty basic implementation.  You have to pick a sound on the sound blocks,
and set up the warning lights' colors and blink settings to your preference before naming them,
as they're a simple on/off switch.
-------------------------------------------------------------------------------------------------------------------------------------

Troubleshooting:
-If you have an exception you need to fix the issue then "remember and exit" and "run" to refresh the system.
-Shield generators bug out sometimes so you may just need to reload the game.
-It is possible you accidentally bugged the code so try deleting the code and reloading it off the workshop.
*/

//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//+++++++++++++++++++++++ Modify below this point at your own risk +++++++++++++++++++++++
//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

public Program()
{
  Runtime.UpdateFrequency = UpdateFrequency.Update10 | UpdateFrequency.Update100;
}

HashSet<string> shieldTypesCython = new HashSet<string>() {
  "SmallShipSmallShieldGeneratorBase",
  "SmallShipMicroShieldGeneratorBase",
  "LargeShipSmallShieldGeneratorBase",
  "LargeShipLargeShieldGeneratorBase"
};

HashSet<string> shieldTypesDStar = new HashSet<string>() {
  "DSControlLarge",
  "DSControlSmall",
  "DSControlTable"
};

List<IMyTerminalBlock> gridBlocks = new List<IMyTerminalBlock>();
List<IMyTextPanel> lcdPanels = new List<IMyTextPanel>();
List<IMySoundBlock> soundBlocks = new List<IMySoundBlock>();
List<IMyLightingBlock> warningLights = new List<IMyLightingBlock>();
List<IMyTerminalBlock> shieldGenList = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> defShieldList = new List<IMyTerminalBlock>();
IMyTerminalBlock shieldGen;
IMyTerminalBlock defShield;
StringBuilder lcdString = new StringBuilder();
int curShieldE,curShieldD,curTotal = 0;
int maxShieldE,maxShieldD,maxTotal = 0;
int percent,percentE,percentD = 0;
int barCounter,barCounterE,barCounterD = 0;
int spsE,spsD,spsTotal = 0;
int lastShield = 0;
int lastSPS = 0;
int refreshCounter = 0;
const string monoBorderA = "\n";
const string monoBorderB = "\n";
const string monoBorderL = "";
const string monoBorderR = "\n";
string[] clcdBGW = new string[] {"","",""};
string[] clcdROYC = new string[] {"","","",""};
string[] rainbowBarChars = new string[] {"","","","","","","","","","","","","","","","","","","","","",""};
int[] shieldOnlyG = {56,57,58,59,63,64,65,68,69,70,72,73,74,75,76,77,78,80,81,82,83,84,85,86,87,89,90,91,98,99,100,101,102,103,216,221,224,226,229,231,233,239,241,248,250,252,259,265,376,379,380,383,385,387,390,392,394,395,396,398,399,400,402,404,405,406,407,408,409,411,413,420,422,423,424,427,537,
  539,542,543,544,546,548,551,553,557,559,563,565,572,574,581,583,586,588,698,701,702,703,707,709,710,711,712,714,718,720,724,726,727,728,729,730,733,735,742,744,747,749,860,865,868,875,879,881,885,891,894,896,903,905,908,910,1022,1023,1024,1027,1029,1031,1032,1033,1034,1036,1040,1042,1046,1048,
  1049,1050,1051,1052,1055,1057,1064,1066,1069,1071,1186,1188,1190,1192,1195,1197,1201,1203,1207,1209,1216,1218,1225,1227,1230,1232,1342,1343,1344,1347,1349,1351,1353,1356,1358,1362,1364,1368,1370,1377,1379,1386,1388,1391,1393,1503,1506,1507,1510,1512,1514,1517,1519,1521,1522,1523,1525,1526,1527,
  1529,1531,1532,1533,1534,1535,1536,1538,1540,1541,1542,1543,1544,1545,1547,1549,1550,1551,1554,1665,1670,1673,1675,1678,1680,1682,1688,1690,1697,1699,1706,1708,1714,1827,1828,1829,1830,1834,1835,1836,1839,1840,1841,1843,1844,1845,1846,1847,1848,1849,1851,1852,1853,1854,1855,1856,1857,1858,1860,
  1861,1862,1863,1864,1865,1866,1867,1869,1870,1871,1872,1873,1874};
int[] shieldOnlyW = {56,57,58,59,64,69,73,74,75,76,77,81,82,83,84,85,86,90,99,100,101,102,103,216,217,220,221,225,230,236,242,251,260,264,265,377,386,391,397,403,412,421,426,538,539,547,552,558,564,573,582,587,700,701,702,703,708,709,710,711,712,713,719,725,726,727,728,729,734,743,748,864,865,869,
  874,880,886,895,904,909,1026,1030,1035,1041,1047,1056,1065,1070,1187,1191,1196,1202,1208,1217,1226,1231,1343,1344,1347,1348,1352,1357,1363,1369,1378,1387,1391,1392,1505,1506,1507,1508,1513,1518,1522,1523,1524,1525,1526,1530,1531,1532,1533,1534,1535,1539,1540,1541,1542,1543,1544,1548,1549,1550,
  1551,1552};
int[] shieldOnlyRotG = {42,43,44,45,46,47,48,49,50,51,52,53,55,56,57,58,59,60,61,62,63,64,65,66,68,69,70,71,72,73,74,75,76,77,78,79,81,82,83,90,91,92,94,95,96,97,98,99,100,101,102,103,104,105,109,110,114,115,116,203,214,216,227,229,240,242,244,251,253,255,266,269,271,274,278,364,366,367,368,369,
  370,371,372,373,375,377,379,380,381,382,383,384,385,386,387,388,390,392,393,394,395,397,398,399,401,403,405,406,407,408,409,410,411,412,414,416,417,418,419,420,421,423,424,425,426,427,429,432,434,437,440,525,527,534,536,538,540,551,553,556,558,560,562,564,575,582,584,590,592,595,597,599,601,686,
  688,695,697,699,701,712,714,717,719,721,723,725,727,728,729,730,731,732,733,734,736,743,745,751,753,756,758,760,762,847,850,851,852,853,854,855,858,860,862,873,875,878,880,882,884,886,888,895,897,899,900,901,902,903,904,906,907,908,909,910,912,915,916,919,920,923,1009,1018,1021,1023,1034,1036,
  1039,1040,1041,1043,1045,1047,1048,1049,1056,1057,1058,1060,1071,1074,1079,1081,1083,1171,1172,1173,1174,1175,1176,1177,1178,1182,1183,1184,1195,1196,1197,1204,1205,1206,1221,1222,1223,1224,1225,1226,1227,1228,1229,1230,1231,1232,1236,1237,1238,1239,1242,1243};
int[] shieldOnlyRotW = {43,44,45,46,47,48,49,50,51,52,56,57,58,59,60,61,62,63,64,65,69,70,71,72,73,74,75,76,77,78,82,91,95,96,97,98,99,100,101,102,103,104,109,114,115,116,204,213,217,230,235,239,243,252,261,269,270,274,275,277,278,365,374,378,391,396,400,404,405,406,407,408,409,410,411,412,413,422,
  430,435,439,526,535,539,552,557,561,565,574,583,591,596,600,687,688,695,696,700,713,718,722,726,735,744,752,753,756,757,760,761,849,850,851,852,853,854,855,856,861,874,883,900,901,902,903,904,905,906,907,908,909,914,915,916,917,921};
int[] energyShieldG = {24,25,26,27,28,29,30,31,33,34,35,38,39,40,42,43,44,45,46,47,48,49,51,52,53,54,55,56,62,63,64,65,69,70,71,73,74,75,86,87,88,89,93,94,95,98,99,100,102,103,104,105,106,107,108,110,111,112,113,114,115,116,117,119,120,121,128,129,130,131,132,133,185,192,194,196,199,201,203,210,
  212,218,222,227,230,232,234,236,246,251,254,256,259,261,263,269,271,278,280,282,289,295,346,348,349,350,351,352,353,355,358,360,362,364,366,367,368,369,370,371,373,375,376,377,380,382,385,386,389,391,393,395,397,406,409,410,413,415,417,420,422,424,425,426,428,429,430,432,434,435,436,437,438,439,
  441,443,450,452,453,454,457,507,509,516,519,521,523,525,527,534,536,539,541,543,545,548,549,550,552,555,558,567,569,572,573,574,576,578,581,583,587,589,593,595,602,604,611,613,616,618,668,670,671,672,673,674,677,679,681,682,684,686,688,689,690,691,692,695,697,698,699,702,704,706,714,716,718,728,
  731,732,733,737,739,740,741,742,744,748,750,754,756,757,758,759,760,763,765,772,774,777,779,829,835,838,840,842,843,845,847,853,856,862,865,867,869,870,871,872,875,879,890,895,898,905,909,911,915,921,924,926,933,935,938,940,990,992,993,994,995,996,999,1001,1002,1004,1006,1008,1010,1011,1012,1013,
  1014,1017,1019,1020,1022,1026,1028,1030,1033,1036,1037,1039,1040,1052,1053,1054,1057,1059,1061,1062,1063,1064,1066,1070,1072,1076,1078,1079,1080,1081,1082,1085,1087,1094,1096,1099,1101,1151,1153,1160,1162,1163,1165,1167,1169,1171,1178,1180,1182,1184,1187,1189,1191,1192,1194,1198,1200,1216,1218,
  1220,1222,1225,1227,1231,1233,1237,1239,1246,1248,1255,1257,1260,1262,1312,1314,1321,1323,1325,1328,1330,1332,1339,1341,1343,1345,1348,1350,1353,1355,1359,1361,1372,1373,1374,1377,1379,1381,1383,1386,1388,1392,1394,1398,1400,1407,1409,1416,1418,1421,1423,1473,1475,1476,1477,1478,1479,1480,1482,
  1484,1486,1489,1491,1493,1494,1495,1496,1497,1498,1500,1502,1505,1507,1509,1512,1513,1516,1520,1522,1533,1536,1537,1540,1542,1544,1547,1549,1551,1552,1553,1555,1556,1557,1559,1561,1562,1563,1564,1565,1566,1568,1570,1571,1572,1573,1574,1575,1577,1579,1580,1581,1584,1634,1641,1643,1645,1648,1650,
  1652,1659,1661,1663,1666,1668,1671,1676,1681,1683,1695,1700,1703,1705,1708,1710,1712,1718,1720,1727,1729,1736,1738,1744,1795,1796,1797,1798,1799,1800,1801,1802,1804,1805,1806,1809,1810,1811,1813,1814,1815,1816,1817,1818,1819,1820,1822,1823,1824,1827,1828,1829,1833,1834,1835,1836,1842,1843,1844,
  1857,1858,1859,1860,1864,1865,1866,1869,1870,1871,1873,1874,1875,1876,1877,1878,1879,1881,1882,1883,1884,1885,1886,1887,1888,1890,1891,1892,1893,1894,1895,1896,1897,1899,1900,1901,1902,1903,1904};
int[] energyShieldW = {25,26,27,28,29,30,34,39,43,44,45,46,47,48,52,53,54,55,56,62,63,64,65,70,74,86,87,88,89,94,99,103,104,105,106,107,111,112,113,114,115,116,120,129,130,131,132,133,186,195,196,200,204,213,217,218,222,223,226,227,231,235,246,247,250,251,255,260,266,272,281,290,294,295,347,356,
  357,361,365,374,379,383,392,393,395,396,407,416,421,427,433,442,451,456,508,517,519,522,526,535,539,540,544,554,556,568,569,577,582,588,594,603,612,617,669,670,671,672,673,678,680,683,687,688,689,690,691,696,697,698,699,700,705,715,716,717,730,731,732,733,738,739,740,741,742,743,749,755,756,757,
  758,759,764,773,778,830,839,842,844,848,857,860,866,870,871,877,894,895,899,904,910,916,925,934,939,991,1000,1003,1005,1009,1018,1022,1027,1032,1038,1056,1060,1065,1071,1077,1086,1095,1100,1152,1161,1165,1166,1170,1179,1183,1188,1193,1199,1217,1221,1226,1232,1238,1247,1256,1261,1313,1322,1326,
  1327,1331,1340,1345,1349,1350,1353,1354,1360,1373,1374,1377,1378,1382,1387,1393,1399,1408,1417,1421,1422,1474,1475,1476,1477,1478,1479,1483,1488,1492,1493,1494,1495,1496,1497,1501,1506,1511,1512,1513,1514,1521,1535,1536,1537,1538,1543,1548,1552,1553,1554,1555,1556,1560,1561,1562,1563,1564,1565,
  1569,1570,1571,1572,1573,1574,1578,1579,1580,1581,1582};
int[] energyShieldRotG = {30,31,32,33,34,35,36,37,38,39,40,41,43,44,45,46,47,48,49,50,51,52,53,54,56,57,58,59,60,61,62,63,64,65,66,67,69,70,71,78,79,80,82,83,84,85,86,87,88,89,90,91,92,93,97,98,102,103,104,119,120,121,122,123,124,125,126,127,128,129,130,191,202,204,215,217,228,230,232,239,241,243,
  254,257,259,262,266,280,291,352,354,355,356,357,358,359,360,361,363,365,367,368,369,370,371,372,373,374,375,376,378,380,381,382,383,385,386,387,389,391,393,394,395,396,397,398,399,400,402,404,405,406,407,408,409,411,412,413,414,415,417,420,422,425,428,441,443,444,445,446,448,449,450,452,513,515,
  522,524,526,528,539,541,544,546,548,550,552,563,570,572,578,580,583,585,587,589,602,604,607,609,611,613,674,676,683,685,687,689,700,702,705,707,709,711,713,715,716,717,718,719,720,721,722,724,731,733,739,741,744,746,748,750,763,765,768,770,772,774,835,838,839,840,841,842,843,846,848,850,861,863,
  866,868,870,872,874,876,883,885,887,888,889,890,891,892,894,895,896,897,898,900,903,904,907,908,911,924,926,929,931,933,935,997,1006,1009,1011,1022,1024,1027,1028,1029,1031,1033,1035,1036,1037,1044,1045,1046,1048,1059,1062,1067,1069,1071,1085,1087,1090,1091,1092,1094,1096,1159,1160,1161,1162,
  1163,1164,1165,1166,1170,1171,1172,1183,1184,1185,1192,1193,1194,1209,1210,1211,1212,1213,1214,1215,1216,1217,1218,1219,1220,1224,1225,1226,1227,1230,1231,1246,1247,1248,1255,1256,1257};
int[] energyShieldRotW = {31,32,33,34,35,36,37,38,39,40,44,45,46,47,48,49,50,51,52,53,57,58,59,60,61,62,63,64,65,66,70,79,83,84,85,86,87,88,89,90,91,92,97,102,103,104,120,121,122,123,124,125,126,127,128,129,192,201,205,218,223,227,231,240,249,257,258,262,263,265,266,281,286,290,353,362,366,379,384,
  388,392,393,394,395,396,397,398,399,400,401,410,418,423,427,442,447,451,514,523,527,540,545,549,553,562,571,579,584,588,603,608,612,675,676,683,684,688,701,706,710,714,723,732,740,741,744,745,748,749,764,769,773,837,838,839,840,841,842,843,844,849,862,871,888,889,890,891,892,893,894,895,896,897,
  902,903,904,905,909,925,934};
int[] defenseShieldG = {19,20,21,22,23,24,28,29,30,31,32,33,34,35,37,38,39,40,41,42,43,44,46,47,48,49,50,51,52,53,55,56,57,60,61,62,66,67,68,69,73,74,75,76,77,78,79,80,91,92,93,94,98,99,100,103,104,105,107,108,109,110,111,112,113,115,116,117,118,119,120,121,122,124,125,126,133,134,135,136,137,138,
  180,186,189,196,198,205,207,214,216,218,221,223,226,231,234,241,251,256,259,261,264,266,268,274,276,283,285,287,294,300,341,343,344,345,348,350,352,353,354,355,356,357,359,361,362,363,364,365,366,368,370,371,372,373,374,375,377,380,382,384,386,389,390,393,395,397,398,399,400,401,402,411,414,415,
  418,420,422,425,427,429,430,431,433,434,435,437,439,440,441,442,443,444,446,448,455,457,458,459,462,502,504,507,509,511,513,520,522,529,531,538,541,543,545,547,549,552,553,554,556,558,572,574,577,578,579,581,583,586,588,592,594,598,600,607,609,616,618,621,623,663,665,668,670,672,674,675,676,677,
  678,681,683,684,685,686,687,690,692,693,694,695,696,699,701,703,704,706,708,711,712,713,717,719,720,721,722,723,733,736,737,738,742,744,745,746,747,749,753,755,759,761,762,763,764,765,768,770,777,779,782,784,824,826,829,831,833,839,842,848,851,857,860,862,864,865,867,870,875,878,884,895,900,903,
  910,914,916,920,926,929,931,938,940,943,945,985,987,990,992,994,996,997,998,999,1000,1003,1005,1006,1007,1008,1009,1012,1014,1015,1016,1017,1018,1021,1023,1024,1026,1028,1032,1033,1034,1037,1039,1041,1042,1043,1044,1045,1057,1058,1059,1062,1064,1066,1067,1068,1069,1071,1075,1077,1081,1083,1084,
  1085,1086,1087,1090,1092,1099,1101,1104,1106,1146,1148,1151,1153,1155,1157,1164,1166,1173,1175,1182,1184,1185,1187,1189,1196,1198,1200,1202,1221,1223,1225,1227,1230,1232,1236,1238,1242,1244,1251,1253,1260,1262,1265,1267,1307,1309,1312,1314,1316,1318,1325,1327,1334,1336,1343,1345,1347,1350,1352,
  1353,1354,1357,1359,1361,1363,1377,1378,1379,1382,1384,1386,1388,1391,1393,1397,1399,1403,1405,1412,1414,1421,1423,1426,1428,1468,1470,1471,1472,1475,1477,1479,1480,1481,1482,1483,1484,1486,1488,1495,1497,1498,1499,1500,1501,1502,1504,1506,1508,1511,1513,1516,1517,1520,1522,1524,1525,1526,1527,
  1528,1529,1538,1541,1542,1545,1547,1549,1552,1554,1556,1557,1558,1560,1561,1562,1564,1566,1567,1568,1569,1570,1571,1573,1575,1576,1577,1578,1579,1580,1582,1584,1585,1586,1589,1629,1635,1638,1645,1647,1649,1656,1663,1665,1667,1670,1672,1675,1680,1683,1690,1700,1705,1708,1710,1713,1715,1717,1723,
  1725,1732,1734,1741,1743,1749,1790,1791,1792,1793,1794,1795,1799,1800,1801,1802,1803,1804,1805,1806,1808,1809,1810,1817,1818,1819,1820,1821,1822,1823,1824,1826,1827,1828,1831,1832,1833,1837,1838,1839,1840,1844,1845,1846,1847,1848,1849,1850,1851,1862,1863,1864,1865,1869,1870,1871,1874,1875,1876,
  1878,1879,1880,1881,1882,1883,1884,1886,1887,1888,1889,1890,1891,1892,1893,1895,1896,1897,1898,1899,1900,1901,1902,1904,1905,1906,1907,1908,1909};
int[] defenseShieldW = {20,21,22,23,24,29,30,31,32,33,34,38,39,40,41,42,43,47,48,49,50,51,52,56,61,66,67,68,69,74,75,76,77,78,79,91,92,93,94,99,104,108,109,110,111,112,116,117,118,119,120,121,125,134,135,136,137,138,181,185,186,190,199,208,217,218,222,226,227,230,231,235,251,252,255,256,260,265,
  271,277,286,295,299,300,342,347,351,360,369,378,379,383,387,396,412,421,426,432,438,447,456,461,503,508,512,521,530,539,541,544,548,549,557,573,574,582,587,593,599,608,617,622,664,669,673,674,675,676,677,682,683,684,685,686,691,692,693,694,695,700,702,705,710,711,712,713,718,719,720,721,722,735,
  736,737,738,743,744,745,746,747,748,754,760,761,762,763,764,769,778,783,825,830,834,843,852,861,864,866,874,875,879,899,900,904,909,915,921,930,939,944,986,991,995,1004,1013,1022,1025,1027,1036,1040,1061,1065,1070,1076,1082,1091,1100,1105,1147,1152,1156,1165,1174,1183,1187,1188,1197,1201,1222,
  1226,1231,1237,1243,1252,1261,1266,1308,1312,1313,1317,1326,1335,1344,1348,1349,1353,1354,1357,1358,1362,1378,1379,1382,1383,1387,1392,1398,1404,1413,1422,1426,1427,1469,1470,1471,1472,1473,1478,1479,1480,1481,1482,1483,1487,1496,1497,1498,1499,1500,1501,1505,1510,1515,1516,1517,1518,1523,1524,
  1525,1526,1527,1528,1540,1541,1542,1543,1548,1553,1557,1558,1559,1560,1561,1565,1566,1567,1568,1569,1570,1574,1575,1576,1577,1578,1579,1583,1584,1585,1586,1587};
int[] defenseShieldRotG = {30,31,32,33,34,35,36,37,38,39,40,41,43,44,45,46,47,48,49,50,51,52,53,54,56,57,58,59,60,61,62,63,64,65,66,67,69,70,71,78,79,80,82,83,84,85,86,87,88,89,90,91,92,93,97,98,102,103,104,119,120,121,122,123,124,125,126,127,128,129,130,191,202,204,215,217,228,230,232,239,241,243,
  254,257,259,262,266,280,291,352,354,355,356,357,358,359,360,361,363,365,367,368,369,370,371,372,373,374,375,376,378,380,381,382,383,385,386,387,389,391,393,394,395,396,397,398,399,400,402,404,405,406,407,408,409,411,412,413,414,415,417,420,422,425,428,441,443,444,445,446,447,448,449,450,452,513,
  515,522,524,526,528,539,541,544,546,548,550,552,563,570,572,578,580,583,585,587,589,602,604,611,613,674,676,683,685,687,689,700,702,705,707,709,711,713,715,716,717,718,719,720,721,722,724,731,733,739,741,744,746,748,750,763,765,772,774,835,838,839,840,841,842,843,846,848,850,861,863,866,868,870,
  872,874,876,883,885,887,888,889,890,891,892,894,895,896,897,898,900,903,904,907,908,911,924,927,928,929,930,931,932,935,997,1006,1009,1011,1022,1024,1027,1028,1029,1031,1033,1035,1036,1037,1044,1045,1046,1048,1059,1062,1067,1069,1071,1086,1095,1159,1160,1161,1162,1163,1164,1165,1166,1170,1171,
  1172,1183,1184,1185,1192,1193,1194,1209,1210,1211,1212,1213,1214,1215,1216,1217,1218,1219,1220,1224,1225,1226,1227,1230,1231,1248,1249,1250,1251,1252,1253,1254,1255};
int[] defenseShieldRotW = {31,32,33,34,35,36,37,38,39,40,44,45,46,47,48,49,50,51,52,53,57,58,59,60,61,62,63,64,65,66,70,79,83,84,85,86,87,88,89,90,91,92,97,102,103,104,120,121,122,123,124,125,126,127,128,129,192,201,205,218,223,227,231,240,249,257,258,262,263,265,266,281,290,353,362,366,379,384,
  388,392,393,394,395,396,397,398,399,400,401,410,418,423,427,442,451,514,523,527,540,545,549,553,562,571,579,584,588,603,612,675,676,683,684,688,701,706,710,714,723,732,740,741,744,745,748,749,764,765,772,773,837,838,839,840,841,842,843,844,849,862,871,888,889,890,891,892,893,894,895,896,897,902,
  903,904,905,909,926,927,928,929,930,931,932,933};

StringBuilder rainbowStripeE = new StringBuilder();
StringBuilder rainbowStripeD = new StringBuilder();
StringBuilder rainbowStripe = new StringBuilder();
StringBuilder rainbowSOBlock = new StringBuilder();
StringBuilder rainbowSORBlock = new StringBuilder();
StringBuilder rainbowESBlock = new StringBuilder();
StringBuilder rainbowESRBlock = new StringBuilder();
StringBuilder rainbowDSBlock = new StringBuilder();
StringBuilder rainbowDSRBlock = new StringBuilder();
StringBuilder roycStripeE = new StringBuilder();
StringBuilder roycStripeD = new StringBuilder();
StringBuilder roycStripe = new StringBuilder();
StringBuilder roycSOBlock = new StringBuilder();
StringBuilder roycSORBlock = new StringBuilder();
StringBuilder roycESBlock = new StringBuilder();
StringBuilder roycESRBlock = new StringBuilder();
StringBuilder roycDSBlock = new StringBuilder();
StringBuilder roycDSRBlock = new StringBuilder();
bool isSetup = false;

bool Setup()
{
  lcdPanels.Clear();
  soundBlocks.Clear();
  warningLights.Clear();
  shieldGenList.Clear();
  defShieldList.Clear();

  GridTerminalSystem.GetBlocks(gridBlocks);

  for (int i = 0; i < gridBlocks.Count; i++) {
    var block = gridBlocks[i];

    if (block is IMyTextPanel && block.CustomName.Contains(lcdNamePrefix))
      lcdPanels.Add(block as IMyTextPanel);
    else if (block is IMySoundBlock && block.CustomName.Contains(shieldAudioName))
      soundBlocks.Add(block as IMySoundBlock);
    else if (block is IMyLightingBlock && block.CustomName.Contains(shieldLightName))
      warningLights.Add(block as IMyLightingBlock);
    else if (shieldTypesCython.Contains(block.BlockDefinition.SubtypeName))
      shieldGenList.Add(block);
    else if (shieldTypesDStar.Contains(block.BlockDefinition.SubtypeName))
      defShieldList.Add(block);
  }

  if (shieldGenList.Count == 0 && defShieldList.Count == 0) {
    Echo("NO\nSHIELD\nGENERATORS\nFOUND");
    return false;
  }

  if (shieldGenList.Count != 0) {
    shieldGen = shieldGenList[0];

    if (!shieldGen.CustomName.Contains(":"))
      shieldGen.CustomName += ":";

    if (!(shieldGen.CustomName.Contains("(") || shieldGen.CustomName.Contains(")"))) {
      Echo("BLOCK NAMED\nIS NOT AN ENERGY\nSHIELD GENERATOR!");
      return false;
    }
  }

  if (defShieldList.Count != 0)
    defShield = defShieldList[0];

  return true;
}

void Main(string argument, UpdateType updateSource) { try { SubMain(argument, updateSource); } catch (Exception e) { var sb = new StringBuilder(); sb.AppendLine("Exception Message:"); sb.AppendLine($"   {e.Message}"); sb.AppendLine(); sb.AppendLine("Stack trace:"); sb.AppendLine(e.StackTrace); sb.AppendLine(); var exceptionDump = sb.ToString(); var lcd = GridTerminalSystem.GetBlockWithName("Debug LCD") as IMyTextPanel; Echo(exceptionDump); Me.CustomData = exceptionDump; lcd?.WritePublicText(exceptionDump, append: false); throw; } }

void SubMain(string argument, UpdateType updateSource)
{
  if (!isSetup) {
    isSetup = Setup();
    return;
  }
  else if ((updateSource & UpdateType.Update100) > 0 && ++refreshCounter > 5)
    isSetup = false;

  curTotal = maxTotal = spsTotal = 0;

  if (lcdPanels.Count == 0)
    Echo($"Warning!\nNo LCD Panels found. Continuing...");

  if (shieldGen != null) {
    SplitString(shieldGen.CustomName, out curShieldE, out maxShieldE);
    spsE = CalcSPS(curShieldE, maxShieldE);
    if (maxShieldE != 0) {
      percentE = (int)(curShieldE/(maxShieldE*0.01));
      barCounterE = (int)((curShieldE*1.54)/(maxShieldE*0.01));
    }
  }

  if (defShield != null) {
    ParseDefShieldInfo(defShield, out curShieldD, out maxShieldD, out spsD);
    if (maxShieldD != 0) {
      percentD = (int)(curShieldD/(maxShieldD*0.01));
      barCounterD = (int)((curShieldD*1.54)/(maxShieldD*0.01));
    }
  }
  if (maxTotal != 0) {
    percent = CalcPercent(curTotal, maxTotal);
    barCounter = (int)((curTotal*1.54)/(maxTotal*0.01));
  }
  rainbowStrip(barCounterE,rainbowStripeE);
  rainbowStrip(barCounterD,rainbowStripeD);
  rainbowStrip(barCounter,rainbowStripe);
  roycStrip(barCounterE,roycStripeE);
  roycStrip(barCounterD,roycStripeD);
  roycStrip(barCounter,roycStripe);
  rainbowSOBlock.Clear();
  rainbowSORBlock.Clear();
  roycSOBlock.Clear();
  roycSORBlock.Clear();
  for (int i=0;i<12;i++) {rainbowSOBlock.Append(rainbowStripe);roycSOBlock.Append(roycStripe);}
  rainbowSORBlock.Append(rainbowSOBlock);
  roycSORBlock.Append(roycSOBlock);
  foreach (int j in shieldOnlyG) {rainbowSOBlock[j]='';roycSOBlock[j]='';}
  foreach (int j in shieldOnlyW) {rainbowSOBlock[j+161]='';roycSOBlock[j+161]='';}
  foreach (int j in shieldOnlyRotG) {rainbowSORBlock[j+322]='';roycSORBlock[j+322]='';}
  foreach (int j in shieldOnlyRotW) {rainbowSORBlock[j+483]='';roycSORBlock[j+483]='';}
  rainbowESBlock.Clear();
  rainbowESRBlock.Clear();
  roycESBlock.Clear();
  roycESRBlock.Clear();
  for (int i=0;i<12;i++) {rainbowESBlock.Append(rainbowStripeE);roycESBlock.Append(roycStripeE);}
  rainbowESRBlock.Append(rainbowESBlock);
  roycESRBlock.Append(roycESBlock);
  foreach (int j in energyShieldG) {rainbowESBlock[j]='';roycESBlock[j]='';}
  foreach (int j in energyShieldW) {rainbowESBlock[j+161]='';roycESBlock[j+161]='';}
  foreach (int j in energyShieldRotG) {rainbowESRBlock[j+322]='';roycESRBlock[j+322]='';}
  foreach (int j in energyShieldRotW) {rainbowESRBlock[j+483]='';roycESRBlock[j+483]='';}
  rainbowDSBlock.Clear();
  rainbowDSRBlock.Clear();
  roycDSBlock.Clear();
  roycDSRBlock.Clear();
  for (int i=0;i<12;i++) {rainbowDSBlock.Append(rainbowStripeD);roycDSBlock.Append(roycStripeD);}
  rainbowDSRBlock.Append(rainbowDSBlock);
  roycDSRBlock.Append(roycDSBlock);
  foreach (int j in defenseShieldG) {rainbowDSBlock[j]='';roycDSBlock[j]='';}
  foreach (int j in defenseShieldW) {rainbowDSBlock[j+161]='';roycDSBlock[j+161]='';}
  foreach (int j in defenseShieldRotG) {rainbowDSRBlock[j+322]='';roycDSRBlock[j+322]='';}
  foreach (int j in defenseShieldRotW) {rainbowDSRBlock[j+483]='';roycDSRBlock[j+483]='';}

  foreach (IMyTextPanel lcdPanel in lcdPanels) {
    if (lcdPanel.CustomName.Contains(lcdModeTDS) || lcdPanel.CustomName.Contains(lcdModeTDL) || lcdPanel.CustomName.Contains(lcdModeBTS) || lcdPanel.CustomName.Contains(lcdModeBTL) || lcdPanel.CustomName.Contains(lcdModeCTD) || lcdPanel.CustomName.Contains(lcdModeCRB) || lcdPanel.CustomName.Contains(lcdModeCCB)) {
      if (lcdPanel.CustomName.Contains(lcdModeTDS) || lcdPanel.CustomName.Contains(lcdModeBTS)) { PrepLCD(lcdPanel, percent, sFontSize);}
      if (lcdPanel.CustomName.Contains(lcdModeTDL) || lcdPanel.CustomName.Contains(lcdModeBTL)) { PrepLCD(lcdPanel, percent, lFontSize);}
      if (lcdPanel.CustomName.Contains(lcdModeTDS) || lcdPanel.CustomName.Contains(lcdModeTDL)) { TextFactory(curTotal, maxTotal, percent, spsTotal, lcdString);}
      if (lcdPanel.CustomName.Contains(lcdModeBTS)) {
        SmBarFactory(curTotal, maxTotal, percent, spsTotal, lcdString);
      }
        else if (lcdPanel.CustomName.Contains(lcdModeBTL)) {
        LgBarFactory(curTotal, maxTotal, percent, spsTotal, lcdString);
      }
      else if (lcdPanel.CustomName.Contains(lcdModeCTD)) {
        PrepLCD(lcdPanel, percent, cFontSize);
        CornerFactory(percent, spsTotal, lcdPanel.CubeGrid.GridSizeEnum.ToString(), lcdString);
      }
      else if (lcdPanel.CustomName.Contains(lcdModeCRB)) {
        lcdPanel.Font="Monospace";
        lcdPanel.FontSize = 0.111f;
        lcdPanel.FontColor = Color.Gray;
        if (lcdPanel.CubeGrid.GridSizeEnum.ToString() == "Large") {
          if (lcdPanel.DetailedInfo.Contains("Corner LCD Flat")) {
            rainbowBarFactory(barCounter,27,lcdString,lcdPanel.CustomData);
          } else {
            rainbowBarFactory(barCounter,24,lcdString,lcdPanel.CustomData);
          }
        }
        else {
          if (lcdPanel.DetailedInfo.Contains("Corner LCD Flat")) {
            rainbowBarFactory(barCounter,50,lcdString,lcdPanel.CustomData);
          } else {
            rainbowBarFactory(barCounter,42,lcdString,lcdPanel.CustomData);
          }
        }
      }
      else if (lcdPanel.CustomName.Contains(lcdModeCCB)) {
        lcdPanel.Font="Monospace";
        lcdPanel.FontSize = 0.111f;
        lcdPanel.FontColor = Color.Gray;
        if (lcdPanel.CubeGrid.GridSizeEnum.ToString() == "Large") {
          if (lcdPanel.DetailedInfo.Contains("Corner LCD Flat")) {
            roycBarFactory(barCounter,27,lcdString,lcdPanel.CustomData);
          } else {
            roycBarFactory(barCounter,24,lcdString,lcdPanel.CustomData);
          }
        }
        else {
          if (lcdPanel.DetailedInfo.Contains("Corner LCD Flat")) {
            roycBarFactory(barCounter,50,lcdString,lcdPanel.CustomData);
          } else {
            roycBarFactory(barCounter,42,lcdString,lcdPanel.CustomData);
          }
        }
      }
      lcdPanel.WritePublicText(lcdString);
      lcdPanel.ShowPublicTextOnScreen();
    }
    else if (lcdPanel.CustomName.Contains(lcdModeGfxH)) {
      ImgNameFactory(percent, "THYA-ShieldH", lcdString);
      SetImageLCD(lcdString.ToString(), lcdPanel);
    }
    else if (lcdPanel.CustomName.Contains(lcdModeGfxV)) {
      ImgNameFactory(percent, "THYA-ShieldV", lcdString);
      SetImageLCD(lcdString.ToString(), lcdPanel);
    }
    else if (lcdPanel.CustomName.Contains(lcdModeGfxC)) {
      ImgNameFactory(percent, "THYA-ShieldC", lcdString);
      SetImageLCD(lcdString.ToString(), lcdPanel);
    }
  }

  foreach (IMySoundBlock soundBlock in soundBlocks) {
    if (percent > shieldAudioWarning) {
      soundBlock.LoopPeriod = 0f;
      soundBlock.Enabled = false;
    }
    else {
      soundBlock.LoopPeriod = float.MaxValue;
      if (!soundBlock.Enabled) {
        soundBlock.Enabled = true;
        soundBlock.Play();
      }
    }
  }

  foreach (IMyLightingBlock warningLight in warningLights) {
    if (percent > shieldVisualWarning) {
      warningLight.Enabled = false;
    }
    else {
      warningLight.Enabled = true;
    }
  }
}

int CalcPercent(int curShields, int maxShields)
{
  if (maxShields == 0) {
    return 0;
  }

  double dPercent = (double)curShields / maxShields;
  Echo($"-------Total Shields-------\n{curShields} / {maxShields}");
  return (int)(Math.Round(dPercent, 2) * 100);
}

void TextFactory(int curShields, int maxShields, int percent, int sps, StringBuilder text)
{
  text.Clear().Append($"    Shields:{percent.ToString()}%\n\n Shields:{FormatNum(curShields)}\n       Full:{FormatNum(maxShields)}\n       S\\s:{sps.ToString()}");
}

void CornerFactory(int percent, int sps, string gridS, StringBuilder text)
{
  text.Clear().Append($"Shield Status: {percent.ToString()}%");
  if (gridS == "Small") {text.Append($"\nCharging: {sps.ToString()} S\\s");}
}

void rainbowBarFactory(int barCounter, int yLimit, StringBuilder text, string mask)
{
  mask=mask.ToUpper();
  int yPos;
  text.Clear();
  text.Append(monoBorderA+monoBorderA+monoBorderB);
  if (mask=="SGDUAL" || mask=="SGDUALR") {
    for (yPos = 0; yPos < ((int)(yLimit-34)/4); yPos++) {text.Append(rainbowStripeD);}
    if (mask=="SGDUALR") {text.Append(rainbowDSRBlock);}else{text.Append(rainbowDSBlock);}
    for (yPos = 0; yPos < ((int)(yLimit-34)/4); yPos++) {text.Append(rainbowStripeD);}
    text.Append(monoBorderB+monoBorderA+monoBorderA+monoBorderB);
    for (yPos = 0; yPos < ((int)(yLimit-34)/4); yPos++) {text.Append(rainbowStripeE);}
    if (mask=="SGDUALR") {text.Append(rainbowESRBlock);}else{text.Append(rainbowESBlock);}
    for (yPos = 0; yPos < ((int)(yLimit-34)/4); yPos++) {text.Append(rainbowStripeE);}
  } else {
    if (mask=="ES" || mask=="ESR") {
      for (yPos = 3; yPos < ((int)(yLimit/2)-6); yPos++) {text.Append(rainbowStripeE);}
      if (mask=="ESR") {text.Append(rainbowESRBlock);}else{text.Append(rainbowESBlock);}
      for (yPos = ((int)(yLimit/2)+6); yPos < (yLimit-3); yPos++) {text.Append(rainbowStripeE);}
    } else if (mask=="DS" || mask=="DSR") {
      for (yPos = 3; yPos < ((int)(yLimit/2)-6); yPos++) {text.Append(rainbowStripeD);}
      if (mask=="DSR") {text.Append(rainbowDSRBlock);}else{text.Append(rainbowDSBlock);}
      for (yPos = ((int)(yLimit/2)+6); yPos < (yLimit-3); yPos++) {text.Append(rainbowStripeD);}
    } else {
      for (yPos = 3; yPos < ((int)(yLimit/2)-6); yPos++) {text.Append(rainbowStripe);}
      if (mask=="SR") {text.Append(rainbowSORBlock);}else{text.Append(rainbowSOBlock);}
      for (yPos = ((int)(yLimit/2)+6); yPos < (yLimit-3); yPos++) {text.Append(rainbowStripe);}
    }
  }
  text.Append(monoBorderB+monoBorderA+monoBorderA);
}

void roycBarFactory(int barCounter, int yLimit, StringBuilder text, string mask)
{
  mask=mask.ToUpper();
  int yPos;
  text.Clear();
  text.Append(monoBorderA+monoBorderA+monoBorderB);
  if (mask=="SGDUAL" || mask=="SGDUALR") {
    for (yPos = 0; yPos < ((int)(yLimit-34)/4); yPos++) {text.Append(roycStripeD);}
    if (mask=="SGDUALR") {text.Append(roycDSRBlock);}else{text.Append(roycDSBlock);}
    for (yPos = 0; yPos < ((int)(yLimit-34)/4); yPos++) {text.Append(roycStripeD);}
    text.Append(monoBorderB+monoBorderA+monoBorderA+monoBorderB);
    for (yPos = 0; yPos < ((int)(yLimit-34)/4); yPos++) {text.Append(roycStripeE);}
    if (mask=="SGDUALR") {text.Append(roycESRBlock);}else{text.Append(roycESBlock);}
    for (yPos = 0; yPos < ((int)(yLimit-34)/4); yPos++) {text.Append(roycStripeE);}
  } else {
    if (mask=="ES" || mask=="ESR") {
      for (yPos = 3; yPos < ((int)(yLimit/2)-6); yPos++) {text.Append(roycStripeE);}
      if (mask=="ESR") {text.Append(roycESRBlock);}else{text.Append(roycESBlock);}
      for (yPos = ((int)(yLimit/2)+6); yPos < (yLimit-3); yPos++) {text.Append(roycStripeE);}
    } else if (mask=="DS" || mask=="DSR") {
      for (yPos = 3; yPos < ((int)(yLimit/2)-6); yPos++) {text.Append(roycStripeD);}
      if (mask=="DSR") {text.Append(roycDSRBlock);}else{text.Append(roycDSBlock);}
      for (yPos = ((int)(yLimit/2)+6); yPos < (yLimit-3); yPos++) {text.Append(roycStripeD);}
    } else {
      for (yPos = 3; yPos < ((int)(yLimit/2)-6); yPos++) {text.Append(roycStripe);}
      if (mask=="SR") {text.Append(roycSORBlock);}else{text.Append(roycSOBlock);}
      for (yPos = ((int)(yLimit/2)+6); yPos < (yLimit-3); yPos++) {text.Append(roycStripe);}
    }
  }
  text.Append(monoBorderB+monoBorderA+monoBorderA);
}

void rainbowStrip(int barCounter, StringBuilder text)
{
  int barPos;
  text.Clear().Append(monoBorderL);
  for (barPos = 0; barPos < 154; barPos++) {
    if (barCounter < barPos || barCounter == 0) {text.Append("");}
    else {
      if (barPos < 22) { text.Append(rainbowBarChars[0]);}
      else{text.Append(rainbowBarChars[(int)((barPos-22)/6)]);}
    }
  }
  text.Append(monoBorderR);
}

void roycStrip(int barCounter, StringBuilder text)
{
  int barPos;
  text.Clear().Append(monoBorderL);
  for (barPos = 0; barPos < 154; barPos++) {
    if (barCounter < barPos || barCounter == 0) {text.Append("");}
    else {
      if ((int)(barCounter/1.54) > shieldHigh) {
        text.Append(clcdROYC[3]);
      }else if ((int)(barCounter/1.54) > shieldLow) {
        text.Append(clcdROYC[2]);
      }else if ((int)(barCounter/1.54) > shieldCrit) {
        text.Append(clcdROYC[1]);
      }else{
        text.Append(clcdROYC[0]);
      }
    }
  }
  text.Append(monoBorderR);
}

void SplitString(string shieldName, out int curShields, out int maxShields)
{
  string[] tempStringArray = shieldName.Split(':');
  string tempString = tempStringArray[1].Trim(' ', '(', ')');
  string[] splitString = tempString.Split('/');

  int.TryParse(splitString[0], out curShields);
  int.TryParse(splitString[1], out maxShields);

  Echo($"------Energy Shields------\n{curShields} / {maxShields}\n");

  curTotal += curShields;
  maxTotal += maxShields;
}

void ParseDefShieldInfo(IMyTerminalBlock shieldBlock, out int curDefShields, out int maxDefShields, out int shieldSPS)
{
  string[] array, tempArray = shieldBlock.CustomInfo.Split('\n');

  if (tempArray[0].StartsWith("[")) {
    array = new string[] { tempArray[0], tempArray[2], tempArray[3] };
    int.TryParse(array[0].Split(' ')[3], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out maxDefShields);
    int.TryParse(array[1].Split(' ')[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out curDefShields);
    int.TryParse(array[2].Split(' ')[3], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out shieldSPS);
    Echo($"-----Defense Shields-----\n{curDefShields} / {maxDefShields}\n");

    curTotal += curDefShields;
    maxTotal += maxDefShields;
    spsTotal += shieldSPS;
  }
  else {
    curDefShields = 0;
    maxDefShields = 0;
    shieldSPS = 0;
  }
}

void PrepLCD(IMyTextPanel lcdPanel, int percent, float font)
{
  lcdPanel.Font = "Debug";
  lcdPanel.FontSize = font;

  if (percent > shieldHigh) {
    lcdPanel.FontColor = txtColHigh;
  }
  else if (percent > shieldLow) {
    lcdPanel.FontColor = txtColMed;
  }
  else if (percent > shieldCrit) {
    lcdPanel.FontColor = txtColLow;
  }
  else {
    lcdPanel.FontColor = txtColCrit;
  }
}

void SmBarFactory(int curShields, int maxShields, int percent, int sps, StringBuilder outSB)
{
  outSB.Clear().Append(" Shields: " + percent.ToString() + "%\n" + lBarSurr);
  percent = percent / 5;
  int i = 0;
  for (; i < 10; i++) {
    if (i < percent) {
      outSB.Append(BarFill);
    }
    else {
      outSB.Append(BarEmpty);
    }
  }
  outSB.Append(rBarSurr + "\n" + lBarSurr);
  for (; i < 20; i++) {
    if (i < percent) {
      outSB.Append(BarFill);
    }
    else {
      outSB.Append(BarEmpty);
    }
  }
  outSB.Append(rBarSurr + "\n\n Shields:" + FormatNum(curShields)
  + "\n Full:" + FormatNum(maxShields) + "\n S\\s:" + sps.ToString());
}

void LgBarFactory(int curShields, int maxShields, int percent, int sps, StringBuilder outSB)
{
  outSB.Clear().Append("            Shields: " + percent.ToString() + "%\n"
    + lBarSurr);
  percent = percent / 5;
  for (int i = 0; i < 20; i++) {
    if (i < percent) {
      outSB.Append(BarFill);
    }
    else {
      outSB.Append(BarEmpty);
    }
  }
  outSB.Append(rBarSurr + "\n          Shields:"
    + FormatNum(curShields) + "\n                Full:"
    + FormatNum(maxShields) + "\n                S\\s:"
    + sps.ToString());
}

void ImgNameFactory(int percent, string prefix, StringBuilder imgName)
{
  imgName.Clear();

  if (percent == 0)
    imgName.Append(prefix + "000");
  else if (percent < 10)
    imgName.Append(prefix + "00" + percent.ToString());
  else if (percent < 100)
    imgName.Append(prefix + "0" + percent.ToString());
  else if (percent == 100)
    imgName.Append(prefix + percent.ToString());
}

void SetImageLCD(string imgName, IMyTextPanel lcdPanel)
{
  if (imgName != lcdPanel.CurrentlyShownImage) {
    lcdPanel.AddImageToSelection(imgName);
    lcdPanel.RemoveImageFromSelection(lcdPanel.CurrentlyShownImage);
  }
  if (lcdPanel.CurrentlyShownImage == null) {
    lcdPanel.AddImageToSelection(imgName);
  }
  lcdPanel.ShowTextureOnScreen();
}

int CalcSPS(int curShields, int maxShields)
{
  int sps = curShields - lastShield;

  if (curShields == maxShields) {
    sps = 0;
    StoreData(curShields, sps);
    return sps;
  }

  if (sps == 0) {
    StoreData(curShields, lastSPS);
    spsTotal += lastSPS;
    return lastSPS;
  }

  StoreData(curShields, sps);
  spsTotal += sps;
  return sps;
}

void StoreData(int curShield, int sps)
{
  lastShield = curShield;
  lastSPS = sps;
}

string FormatNum(int num)
{
  if (num >= 1000000000)
    return (num / 1000000000D).ToString("0.##b");

  if (num >= 1000000)
    return (num / 1000000D).ToString("0.##m");

  if (num >= 100000)
    return (num / 1000D).ToString("0.#k");

  if (num >= 1000)
    return (num / 1000D).ToString("0.##k");

  return num.ToString("N0");
}
