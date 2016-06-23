using System.Collections.Generic;

/*
 
    SEE https://en.wikipedia.org/wiki/Enigma_rotor_details
 
    I		EKMFLGDQVZNTOWYHXUSPAIBRCJ	1930				Enigma I
    II		AJDKSIRUXBLHWTMCQGZNPYFVOE	1930				Enigma I
    III		BDFHJLCPRTXVZNYEIWGAKMUSQO	1930				Enigma I
    IV		ESOVPZJAYQUIRHXLNFTGKDCMWB	December 1938		M3 Army
    V		VZBRGITYUPSDNHLXAWMJQOFECK	December 1938		M3 Army
    VI		JPGVOUMFYQBENHZRDKASXLICTW	1939				M3 & M4 Naval (FEB 1942)
    VII		NZJHGRCXMYSWBOUFAIVLPEKQDT	1939				M3 & M4 Naval (FEB 1942)
    VIII	FKQHTLXOCBJSPDZRAMEWNIUYGV	1939				M3 & M4 Naval (FEB 1942)

    Beta				LEYJVCNIXWPBQMDRTAKZGFUHOS	Spring 1941	M4 R2
    Gamma				FSOKANUERHMBTIYCWLQPZXVGJD	Spring 1942	M4 R2
    Reflector B			YRUHQSLDPXNGOKMIEBFZCWVJAT		
    Reflector C			FVPJIAOYEDRZXWGCTKUQSBNMHL		
    Reflector B Thin	ENKQAUYWJICOPBLMDXZVFTHRGS	1940	M4 R1 (M3 + Thin)
    Reflector C Thin	RDOBJNTKVEHMLFCWZAXGYIPSUQ	1940	M4 R1 (M3 + Thin)

    I	Q	If rotor steps from Q to R, the next rotor is advanced
    II	E	If rotor steps from E to F, the next rotor is advanced
    III	V	If rotor steps from V to W, the next rotor is advanced
    IV	J	If rotor steps from J to K, the next rotor is advanced
    V	Z	If rotor steps from Z to A, the next rotor is advanced
    VI, VII, VIII	Z+M	If rotor steps from Z to A, or from M to N the next rotor is advanced

*/

namespace Enigma
{
    internal static class Constants
    {
        public const string ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static List<string> Rotors = new List<string>()
        {
            "EKMFLGDQVZNTOWYHXUSPAIBRCJ",
            "AJDKSIRUXBLHWTMCQGZNPYFVOE",
            "BDFHJLCPRTXVZNYEIWGAKMUSQO",
            "ESOVPZJAYQUIRHXLNFTGKDCMWB",
            "VZBRGITYUPSDNHLXAWMJQOFECK",
            "JPGVOUMFYQBENHZRDKASXLICTW",
            "NZJHGRCXMYSWBOUFAIVLPEKQDT",
            "FKQHTLXOCBJSPDZRAMEWNIUYGV",
            "LEYJVCNIXWPBQMDRTAKZGFUHOS",
            "FSOKANUERHMBTIYCWLQPZXVGJD"
        };

        public static List<string> Reflectors = new List<string>()
        {
            "YRUHQSLDPXNGOKMIEBFZCWVJAT",
            "FVPJIAOYEDRZXWGCTKUQSBNMHL",
            "ENKQAUYWJICOPBLMDXZVFTHRGS",
            "RDOBJNTKVEHMLFCWZAXGYIPSUQ"
        };

        public static List<int[]> Notches = new List<int[]>()
        {
            new int[] {Constants.ALPHABET.IndexOf('Q')},
            new int[] {Constants.ALPHABET.IndexOf('E')},
            new int[] {Constants.ALPHABET.IndexOf('V')},
            new int[] {Constants.ALPHABET.IndexOf('J')},
            new int[] {Constants.ALPHABET.IndexOf('Z')},
            new int[] {Constants.ALPHABET.IndexOf('M'), Constants.ALPHABET.IndexOf('Z')},
            new int[] {Constants.ALPHABET.IndexOf('M'), Constants.ALPHABET.IndexOf('Z')},
            new int[] {Constants.ALPHABET.IndexOf('M'), Constants.ALPHABET.IndexOf('Z')},
            new int[0],
            new int[0]
        };
    }
}
