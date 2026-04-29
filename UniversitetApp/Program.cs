using System;
using System.Collections.Generic;
using System.Linq;

// --- 1. MODELLER ---

public abstract class Bruker
{
    public string Navn { get; set; }
    public string Brukernavn { get; set; }
    public string Passord { get; set; }
    public string Rolle { get; set; } // "Student", "Lærer", "Bibliotekar"
}

public class Student : Bruker
{
    public string StudentID { get; set; }
    public List<string> Karakterer { get; set; } = new List<string>(); 
}

public class Faglaerer : Bruker
{
    public string AnsattID { get; set; }
    public string Avdeling { get; set; }
}

public class Bibliotekar : Bruker
{
    public string AnsattID { get; set; }
}

public class Kurs
{
    public string Kode { get; set; }
    public string Navn { get; set; }
    public int Studiepoeng { get; set; }
    public int MaksPlasser { get; set; } // Implementert sjekk på denne
    public string Pensum { get; set; } = "Ikke registrert";
    public List<Student> Deltakere { get; set; } = new List<Student>();
    public Dictionary<string, string> KarakterBok { get; set; } = new Dictionary<string, string>(); 
}

public class Bok
{
    public string Tittel { get; set; }
    public int Antall { get; set; }
    public int Utlaant { get; set; }
}

public class Laan
{
    public Bok Bok { get; set; }
    public Bruker Laantaker { get; set; }
    public bool ErAktiv { get; set; } = true;
}

// --- 2. HOVEDSYSTEM ---

public class Program
{
    public static List<Bruker> alleBrukere = new List<Bruker>();
    public static List<Kurs> alleKurs = new List<Kurs>();
    public static List<Bok> alleBoker = new List<Bok>();
    public static List<Laan> utlaanHistorikk = new List<Laan>();
    public static Bruker innloggetBruker = null;

    public static void Main(string[] args)
    {
        SeedData();
        StartMeny();
    }

    public static void StartMeny()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== VELKOMMEN TIL UNIVERSITETET ===");
            Console.WriteLine("[1] Logg inn");
            Console.WriteLine("[2] Registrer ny bruker");
            Console.WriteLine("[0] Avslutt");
            
            string valg = Console.ReadLine();
            if (valg == "1") LoggInn();
            else if (valg == "2") RegistrerBruker();
            else if (valg == "0") break;
        }
    }

    public static void LoggInn()
    {
        Console.Write("Brukernavn: "); string bn = Console.ReadLine();
        Console.Write("Passord: "); string ps = Console.ReadLine();

        innloggetBruker = alleBrukere.FirstOrDefault(u => u.Brukernavn == bn && u.Passord == ps);

        if (innloggetBruker != null) HovedMeny();
        else { Console.WriteLine("Feil brukernavn eller passord!"); Console.ReadKey(); }
    }

    public static void RegistrerBruker()
    {
        Console.WriteLine("Velg rolle: [1] Student [2] Lærer [3] Bibliotekar");
        string rolleValg = Console.ReadLine();
        
        Console.Write("Fullt navn: "); string navn = Console.ReadLine();
        Console.Write("Velg brukernavn: "); string bn = Console.ReadLine();
        Console.Write("Velg passord: "); string ps = Console.ReadLine();

        if (rolleValg == "1") alleBrukere.Add(new Student { Navn = navn, Brukernavn = bn, Passord = ps, Rolle = "Student" });
        else if (rolleValg == "2") alleBrukere.Add(new Faglaerer { Navn = navn, Brukernavn = bn, Passord = ps, Rolle = "Lærer" });
        else if (rolleValg == "3") alleBrukere.Add(new Bibliotekar { Navn = navn, Brukernavn = bn, Passord = ps, Rolle = "Bibliotekar" });
        
        Console.WriteLine("Bruker registrert! Du kan nå logge inn.");
        Console.ReadKey();
    }

    public static void HovedMeny()
    {
        bool loggetInn = true;
        while (loggetInn)
        {
            Console.Clear();
            Console.WriteLine($"Innlogget som: {innloggetBruker.Navn} ({innloggetBruker.Rolle})");
            Console.WriteLine("-----------------------------------");

            if (innloggetBruker.Rolle == "Student") StudentMeny();
            else if (innloggetBruker.Rolle == "Lærer") LaererMeny();
            else if (innloggetBruker.Rolle == "Bibliotekar") BibliotekMeny();

            Console.WriteLine("[0] Logg ut");
            string valg = Console.ReadLine();
            if (valg == "0") loggetInn = false;
            else UtforHandling(valg);
        }
    }

    public static void StudentMeny()
    {
        Console.WriteLine("[1] Meld på kurs");
        Console.WriteLine("[2] Meld av kurs");
        Console.WriteLine("[3] Se mine karakterer");
        Console.WriteLine("[4] Søk/Lån bok");
        Console.WriteLine("[5] Lever bok");
    }

    public static void LaererMeny()
    {
        Console.WriteLine("[1] Opprett kurs");
        Console.WriteLine("[2] Sett karakter");
        Console.WriteLine("[3] Registrer pensum");
        Console.WriteLine("[4] Søk kurs/bøker"); // Nå implementert
        Console.WriteLine("[5] Lån/Lever bok");
    }

    public static void BibliotekMeny()
    {
        Console.WriteLine("[1] Registrer ny bok");
        Console.WriteLine("[2] Se aktive lån");
        Console.WriteLine("[3] Se historikk");
    }

    public static void UtforHandling(string valg)
    {
        try {
            if (innloggetBruker.Rolle == "Student")
            {
                if (valg == "1") MeldPaaKurs();
                if (valg == "3") SeKarakterer();
                if (valg == "4") LaanBok();
                if (valg == "5") ReturnerBok();
            }
            else if (innloggetBruker.Rolle == "Lærer")
            {
                if (valg == "1") OpprettKurs();
                if (valg == "2") SettKarakter();
                if (valg == "3") RegistrerPensum();
                if (valg == "4") SokKursOgBoker(); // Ny funksjon
                if (valg == "5") LaanBok(); 
            }
            else if (innloggetBruker.Rolle == "Bibliotekar")
            {
                if (valg == "1") RegistrerBok();
                if (valg == "2") VisHistorikk();
            }
        } catch (Exception ex) {
            Console.WriteLine($"En feil oppstod: {ex.Message}");
        }
        Console.WriteLine("\nTrykk en tast..."); Console.ReadKey();
    }

    // --- FORBEDREDE FUNKSJONER ---

    public static void OpprettKurs()
    {
        Console.Write("Kurskode: "); string kode = Console.ReadLine();
        if (alleKurs.Any(k => k.Kode.Equals(kode, StringComparison.OrdinalIgnoreCase)))
        {
            throw new Exception("Kurskoden eksisterer allerede!");
        }
        Kurs k = new Kurs { Kode = kode };
        Console.Write("Navn: "); k.Navn = Console.ReadLine();
        Console.Write("Maks plasser: "); k.MaksPlasser = int.Parse(Console.ReadLine()); // Lagt til inndata for kapasitet
        alleKurs.Add(k);
        Console.WriteLine("Kurs opprettet.");
    }

    public static void MeldPaaKurs()
    {
        Console.Write("Kurskode: "); string kode = Console.ReadLine();
        var kurs = alleKurs.FirstOrDefault(k => k.Kode.Equals(kode, StringComparison.OrdinalIgnoreCase));
        var student = (Student)innloggetBruker;

        if (kurs != null) {
            // SJEKK 1: Er studenten allerede påmeldt?
            if (kurs.Deltakere.Contains(student)) throw new Exception("Du er allerede påmeldt!");

            // SJEKK 2: Er kurset fullt? (NY)
            if (kurs.Deltakere.Count >= kurs.MaksPlasser) 
            {
                throw new Exception($"Kurset er fullt! Maks kapasitet er {kurs.MaksPlasser} plasser.");
            }

            kurs.Deltakere.Add(student);
            Console.WriteLine($"Påmeldt! ({kurs.Deltakere.Count}/{kurs.MaksPlasser} plasser brukt)");
        }
        else {
            Console.WriteLine("Fant ikke kurset.");
        }
    }

    // NY FUNKSJON: Søk for faglærer
    public static void SokKursOgBoker()
    {
        Console.Write("Skriv søkeord (kursnavn eller boktittel): ");
        string sok = Console.ReadLine().ToLower();

        Console.WriteLine("\n--- SØKERESULTATER KURS ---");
        var kursTreff = alleKurs.Where(k => k.Navn.ToLower().Contains(sok) || k.Kode.ToLower().Contains(sok)).ToList();
        if (kursTreff.Any()) {
            foreach (var k in kursTreff)
                Console.WriteLine($"- {k.Navn} ({k.Kode}) | Plasser: {k.Deltakere.Count}/{k.MaksPlasser}");
        } else Console.WriteLine("Ingen kurs funnet.");

        Console.WriteLine("\n--- SØKERESULTATER BØKER ---");
        var bokTreff = alleBoker.Where(b => b.Tittel.ToLower().Contains(sok)).ToList();
        if (bokTreff.Any()) {
            foreach (var b in bokTreff)
                Console.WriteLine($"- {b.Tittel} | Tilgjengelig: {b.Antall - b.Utlaant}/{b.Antall}");
        } else Console.WriteLine("Ingen bøker funnet.");
    }

    // --- ØVRIGE FUNKSJONER ---

    public static void SettKarakter()
    {
        Console.Write("Kurskode: "); string kode = Console.ReadLine();
        var kurs = alleKurs.FirstOrDefault(k => k.Kode == kode);
        if (kurs == null) return;

        Console.Write("StudentID: "); string sid = Console.ReadLine();
        Console.Write("Karakter (A-F): "); string karakter = Console.ReadLine();
        
        kurs.KarakterBok[sid] = karakter;
        Console.WriteLine("Karakter registrert.");
    }

    public static void RegistrerPensum()
    {
        Console.Write("Kurskode: "); string kode = Console.ReadLine();
        var kurs = alleKurs.FirstOrDefault(k => k.Kode == kode);
        if (kurs != null) {
            Console.Write("Pensum-tittel: ");
            kurs.Pensum = Console.ReadLine();
        }
    }

    public static void SeKarakterer()
    {
        var student = (Student)innloggetBruker;
        foreach(var kurs in alleKurs) {
            if (kurs.KarakterBok.ContainsKey(student.Navn)) 
                Console.WriteLine($"{kurs.Navn}: {kurs.KarakterBok[student.Navn]}");
        }
    }

    public static void LaanBok()
    {
        Console.Write("Boktittel: "); string t = Console.ReadLine();
        var bok = alleBoker.FirstOrDefault(b => b.Tittel.Equals(t, StringComparison.OrdinalIgnoreCase));
        if (bok != null && bok.Utlaant < bok.Antall) {
            bok.Utlaant++;
            utlaanHistorikk.Add(new Laan { Bok = bok, Laantaker = innloggetBruker });
            Console.WriteLine("Bok lånt!");
        } else {
            Console.WriteLine("Boken er enten ikke funnet eller alle eksemplarer er utlånt.");
        }
    }

    public static void ReturnerBok()
    {
        Console.Write("Boktittel: "); string t = Console.ReadLine();
        var laan = utlaanHistorikk.FirstOrDefault(l => l.Bok.Tittel.Equals(t, StringComparison.OrdinalIgnoreCase) && l.Laantaker == innloggetBruker && l.ErAktiv);
        if (laan != null) {
            laan.ErAktiv = false;
            laan.Bok.Utlaant--;
            Console.WriteLine("Bok returnert!");
        }
    }

    public static void RegistrerBok()
    {
        Console.Write("Tittel: "); string t = Console.ReadLine();
        Console.Write("Antall: "); int a = int.Parse(Console.ReadLine());
        alleBoker.Add(new Bok { Tittel = t, Antall = a });
    }

    public static void VisHistorikk()
    {
        foreach(var l in utlaanHistorikk)
            Console.WriteLine($"{l.Laantaker.Navn} - {l.Bok.Tittel} (Aktiv: {l.ErAktiv})");
    }

    public static void SeedData()
    {
        alleBrukere.Add(new Faglaerer { Navn = "Admin Lærer", Brukernavn = "laerer", Passord = "123", Rolle = "Lærer" });
        alleBrukere.Add(new Student { Navn = "Ola", Brukernavn = "student", Passord = "123", Rolle = "Student" });
        alleBrukere.Add(new Bibliotekar { Navn = "Berit", Brukernavn = "bib", Passord = "123", Rolle = "Bibliotekar" });
        
        alleBoker.Add(new Bok { Tittel = "C# Manual", Antall = 5 });
        
        // La til et kurs med begrenset kapasitet for testing
        alleKurs.Add(new Kurs { Kode = "IT101", Navn = "Programmering", MaksPlasser = 2 });
    }
}