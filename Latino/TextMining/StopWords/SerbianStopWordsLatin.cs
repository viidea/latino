﻿/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    SerbianStopWordsLatin.cs
 *  Desc:    Serbian stop words (latin)
 *  Created: Mar-2010
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

namespace Latino.TextMining
{
    /* .-----------------------------------------------------------------------
       |
       |  Class StopWords
       |
       '-----------------------------------------------------------------------
    */
    public static partial class StopWords
    {
        // this list is taken from http://www.filewatcher.com/p/punbb-1.2.14.tbz.550363/www/punbb/upload/lang/Serbian/stopwords.txt.html
        public static Set<string>.ReadOnly SerbianStopWordsLatin
            = new Set<string>.ReadOnly(new Set<string>(new string[] {
                "baš",
                "bez",
                "biće",
                "bio",
                "biti",
                "blizu",
                "broj",
                "dana",
                "danas",
                "doći",
                "dobar",
                "dobiti",
                "dok",
                "dole",
                "došao",
                "drugi",
                "duž",
                "dva",
                "često",
                "čiji",
                "gde",
                "gore",
                "hvala",
                "ići",
                "iako",
                "ide",
                "ima",
                "imam",
                "imao",
                "ispod",
                "između",
                "iznad",
                "izvan",
                "izvoli",
                "jedan",
                "jedini",
                "jednom",
                "jeste",
                "još",
                "juče",
                "kad",
                "kako",
                "kao",
                "koga",
                "koja",
                "koje",
                "koji",
                "kroz",
                "mali",
                "manji",
                "misli",
                "mnogo",
                "moći",
                "mogu",
                "mora",
                "morao",
                "naći",
                "naš",
                "negde",
                "nego",
                "nekad",
                "neki",
                "nemam",
                "nešto",
                "nije",
                "nijedan",
                "nikada",
                "nismo",
                "ništa",
                "njega",
                "njegov",
                "njen",
                "njih",
                "njihov",
                "oko",
                "okolo",
                "ona",
                "onaj",
                "oni",
                "ono",
                "osim",
                "ostali",
                "otišao",
                "ovako",
                "ovamo",
                "ovde",
                "ove",
                "ovo",
                "pitati",
                "početak",
                "pojedini",
                "posle",
                "povodom",
                "praviti",
                "pre",
                "preko",
                "prema",
                "prvi",
                "put",
                "radije",
                "sada",
                "smeti",
                "šta",
                "stvar",
                "stvarno",
                "sutra",
                "svaki",
                "sve",
                "svim",
                "svugde",
                "tačno",
                "tada",
                "taj",
                "takođe",
                "tamo",
                "tim",
                "učinio",
                "učiniti",
                "umalo",
                "unutra",
                "upotrebiti",
                "uzeti",
                "vaš",
                "većina",
                "veoma",
                "video",
                "više",
                "zahvaliti",
                "zašto",
                "zbog",
                "želeo",
                "želi",
                "znati"}));
    }
}
