using Xunit;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;

public class UniversitetTester
{
    // Hjelpemetode for å tømme listene før hver test så de er isolerte
    private void NullstillSystem()
    {
        Program.alleKurs.Clear();
        Program.alleBoker.Clear();
        Program.alleBrukere.Clear();
        Program.utlaanHistorikk.Clear();
        Program.innloggetBruker = null;
    }

    [Fact]
    public void MeldPaaKurs_SkalHindrePaamelding_NarKursetErFullt()
    {
        // 1. Arrange
        NullstillSystem();
        var kurs = new Kurs { Kode = "TEST101", Navn = "Testkurs", MaksPlasser = 1 };
        Program.alleKurs.Add(kurs);
        
        var student1 = new Student { Navn = "Student 1", Brukernavn = "s1" };
        var student2 = new Student { Navn = "Student 2", Brukernavn = "s2" };
        
        // Simulerer at student 1 logger inn og melder seg på
        Program.innloggetBruker = student1;
        
        // Vi simulerer inndata (kurskode) ved å bruke StringReader
        Console.SetIn(new StringReader("TEST101")); 

        // 2. Act
        Program.MeldPaaKurs(); // Første påmelding går OK

        // Prøver påmelding nr 2 (som skal kaste exception pga kapasitet)
        Program.innloggetBruker = student2;
        Console.SetIn(new StringReader("TEST101"));

        // 3. Assert
        var exception = Assert.Throws<Exception>(() => Program.MeldPaaKurs());
        Assert.Contains("Kurset er fullt", exception.Message);
        Assert.Single(kurs.Deltakere);
    }

    [Fact]
    public void MeldPaaKurs_SkalHindreDuplikatPaamelding()
    {
        // Arrange
        NullstillSystem();
        var kurs = new Kurs { Kode = "PROG", Navn = "Programmering", MaksPlasser = 10 };
        Program.alleKurs.Add(kurs);
        
        var student = new Student { Navn = "Ola", Brukernavn = "ola1" };
        Program.innloggetBruker = student;

        // Act & Assert
        Console.SetIn(new StringReader("PROG"));
        Program.MeldPaaKurs(); // Første gang OK

        Console.SetIn(new StringReader("PROG"));
        var exception = Assert.Throws<Exception>(() => Program.MeldPaaKurs());
        
        Assert.Equal("Du er allerede påmeldt!", exception.Message);
    }

    [Fact]
    public void SokKursOgBoker_SkalFinneRiktigData()
    {
        // Arrange
        NullstillSystem();
        Program.alleKurs.Add(new Kurs { Kode = "MAT100", Navn = "Matematikk 1", MaksPlasser = 5 });
        Program.alleBoker.Add(new Bok { Tittel = "Matematikk for alle", Antall = 3 });
        
        // Simulerer at brukeren søker på "matte"
        Console.SetIn(new StringReader("matematikk"));
        
        // Vi fanger konsoll-output for å sjekke om søket viser resultatene
        var output = new StringWriter();
        Console.SetOut(output);

        // 2. Act
        Program.SokKursOgBoker();

        // 3. Assert
        string result = output.ToString();
        Assert.Contains("Matematikk 1", result);
        Assert.Contains("Matematikk for alle", result);
    }

    [Fact]
    public void LaanBok_SkalOekeUtlaantAntall()
    {
        // Arrange
        NullstillSystem();
        var bok = new Bok { Tittel = "C# Boka", Antall = 1, Utlaant = 0 };
        Program.alleBoker.Add(bok);
        Program.innloggetBruker = new Student { Navn = "Låner" };

        Console.SetIn(new StringReader("C# Boka"));

        // 2. Act
        Program.LaanBok();

        // 3. Assert
        Assert.Equal(1, bok.Utlaant);
        Assert.Single(Program.utlaanHistorikk);
    }
}