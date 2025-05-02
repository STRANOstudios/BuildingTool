# Building Tool – Editor per Edifici Modulari in Unity

## Consegna

**Progetto:** Building Tool  
**Obiettivo:** Scrivere un tool che permetta con facilità di assemblare un edificio modulare, con mura, pavimento, tetto, in multipli piani.

### Requisiti richiesti

- [x] GUI da cui l'utente può selezionare i moduli da utilizzare  
- [x] Preview dell'asset in scena in corrispondenza del cursore, con colori diversi in base alla validità della posizione  
- [x] Possibilità di piazzare il modulo solo se la posizione è valida  
- [x] Snap del modulo al modulo più vicino già piazzato  
- [x] Possibilità di salvare gli edifici prodotti tramite il tool

## Descrizione

**Building Tool** è un’estensione per l’Editor di Unity pensata per progettare edifici modulari 3D in modo interattivo. Permette di selezionare prefab, piazzarli con anteprima visiva, usarne lo snapping intelligente e salvarli come prefab unificato.

## Funzionalità principali

- Interfaccia grafica per selezionare moduli da pacchetti prefab (pavimenti, pareti, tetti)
- Preview visiva del modulo sotto il cursore, con colore verde/rosso per validità
- Smart Snap tra moduli adiacenti (attivo di default, disattivabile con `Alt`)
- Snap globale Unity (Editor Snap – `Ctrl`)
- Rotazione moduli con la rotella del mouse
- Costruzione su multipli piani regolabili con uno slider
- Controllo collisioni in tempo reale (disattivabile con `Shift`)
- Esportazione prefab dell'intera struttura

## Come si usa

1. **Apri il tool**:  
   Vai in `Tools > Building Tool > Launch Tool`

2. **Crea un pacchetto**:  
   Nella finestra *Pack Manager*, definisci i tuoi prefab modulari (Floors, Walls, Roofs)

3. **Seleziona e posiziona i moduli**:  
   Nella finestra *Builder Tool*, seleziona un modulo e posizionalo nella scena con anteprima

4. **Salva l’edificio**:  
   Clicca `Save Structure as Prefab` per generare un prefab dell’intera struttura

## Comandi rapidi (Tutorial)

```text
- Left Click: Posiziona il modulo selezionato
- Right Click: Annulla la selezione corrente
- Mouse Wheel: Ruota il modulo in base ai gradi impostati nel pannello di snap di unity
- Plane Height: Modifica l’altezza tramite lo slider
- Smart Snap: Attivo per default, tieni premuto ALT per disattivarlo temporaneamente
- Grid Snap: Usa lo snap globale di Unity (CTRL)
- Collisioni: Attive per default, tieni premuto SHIFT per ignorarle temporaneamente

Quando hai finito, clicca 'Save Structure as Prefab' per salvare l'edificio.
```
