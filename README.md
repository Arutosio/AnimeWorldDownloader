<p align="center">
  <img src="animeWorldDownloader.png" alt="AnimeWorldDownloader" width="160">
</p>

# AnimeWorldDownloader (.づ◡﹏◡)づ.

[![Ecchi Style](https://static-cdn.jtvnw.net/jtv_user_pictures/panel-55778697-image-aaa18660-1043-413a-a788-dad202eac409)](https://discord.gg/K9NHNrx)
[![Ecchi Style](https://static-cdn.jtvnw.net/jtv_user_pictures/panel-55778697-image-21f6cc49-c9fd-4ae5-8224-c901119f1505)](https://streamlabs.com/arutosio)
[![Build Status](https://dev.azure.com/Arutosio/Arutosio/_apis/build/status/Arutosio.AnimeWorldDownloader?branchName=master)](https://dev.azure.com/Arutosio/Arutosio/_build/latest?definitionId=3&branchName=master)

Applicazione desktop per cercare, guardare e scaricare anime da AnimeWorld.

Link Download: [AnimeWorldDownloader Releases](https://github.com/Arutosio/AnimeWorldDownloader/releases)

## Requisiti

- Windows 10/11
- .NET 10 SDK

## Progetti

| Progetto | Descrizione |
|----------|-------------|
| **AnimeWorldDownloader_App** | App MAUI con interfaccia grafica (ricerca, streaming, download) |
| **AnimeWorldDownloader_Console** | Versione console legacy (.NET Framework 4.8) |

## Guida all'utilizzo (App MAUI)

### 1. Ricerca anime
- Avvia l'app e inserisci il nome dell'anime nel campo di ricerca
- Premi Invio o clicca "Cerca"
- Seleziona un anime dalla lista dei risultati e clicca "Info Page"

### 2. Dettaglio anime
- Visualizza le informazioni dell'anime (stato, episodi, genere, durata, descrizione)
- Clicca "Download" per andare alla pagina degli episodi

### 3. Episodi
Ogni episodio ha due azioni:
- **Play** (bottone verde) — apre il player video integrato per guardare l'episodio in streaming
- **Scarica** (bottone blu) — scarica l'episodio singolarmente

### 4. Selezione multipla e download batch
- Usa le **checkbox** per selezionare gli episodi da scaricare
- "Seleziona tutti" / "Deseleziona" per gestire la selezione rapidamente
- Clicca "Scarica selezionati (N)" per avviare il download degli episodi selezionati

### 5. Gestione download
- Il bottone **"DL (N)"** in alto a destra mostra/nasconde il pannello dei download attivi
- Ogni download mostra: numero episodio, stato, barra di progresso e percorso del file
- Bottone **X** per annullare un singolo download
- "Annulla tutti" per fermare tutti i download in corso

### 6. Cartella di salvataggio
- Gli episodi vengono salvati in: `[Cartella base]\NomeAnime (Anno)\`
- Il nome dei file segue il formato: `Nome_Anime_Ep_01.mp4`
- La cartella base di default e': `Desktop\AnimeDownloads\`
- Clicca **"Apri cartella"** per aprire la cartella nel file explorer
- Clicca **"Cambia cartella"** per configurare un percorso diverso (la scelta viene ricordata tra le sessioni)

### Esempio struttura file
```
Desktop\AnimeDownloads\
  └── That Time I Got Reincarnated as a Slime (2018)\
      ├── That_Time_I_Got_Reincarnated_as_a_Slime_Ep_01.mp4
      ├── That_Time_I_Got_Reincarnated_as_a_Slime_Ep_02.mp4
      └── ...
```

## Build

```bash
cd AnimeWorldDownloader_App
dotnet build
dotnet run
```

## Tecnologie

- .NET 10 MAUI (Windows)
- AngleSharp (HTML parsing)
- CommunityToolkit.Maui.MediaElement (player video)
