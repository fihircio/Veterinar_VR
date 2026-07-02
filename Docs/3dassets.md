# 3D Assets Catalog & Requirements

This document tracks current, locked, and pending 3D/audio assets for the **Veterinar_VR** project, along with optimization guides.

---

## Existing Assets Inventory

| Asset Class / Name | Scene Usage | Priority | Animated? | Cost / Source Link | License Type | Status |
|---|---|---|---|---|---|---|
| **Cow Realistic (Red Deer)** | S02, S03 | Required | Yes | [$25 / Unity Asset Store](https://assetstore.unity.com/packages/3d/characters/animals/mammals/cow-realistic-82765) | Standard Unity | **LOCKED** |
| **Cattle Crush / Squeeze Chute** | S03, S04 | Required | No | [Free / Kathyjane / 3D Warehouse](https://embed-3dwarehouse-classic.sketchup.com/model/d96dff8d4f2ff04416ab5bf7d4b9ff0c/Cattle-Crush) | Royalty-Free | **LOCKED** |
| **Farm Megapack (Environments)** | S02, S03, S04 | Required | No | [$49.99 / Static Soul Studio](https://assetstore.unity.com/packages/3d/environments/farm-megapack-buildings-barns-and-props-371044) | Standard Unity | **LOCKED** |
| **Hospital Props HQ (Vet Dressing)** | S04 | Required | No | [$9.95 / VIS Games](https://assetstore.unity.com/packages/3d/props/interior/hospital-props-collection-549) | Standard Unity | **LOCKED** |
| **Industrial & Office Interiors** | S05, S06 | Required | No | [$23.00 / Studio CyFi via ArtStation](https://www.fab.com/listings/8eb5acc8-4e1a-4f5e-9b4e-1a1b981bf0d8) | Commercial | **LOCKED** |
| **Office Props Set – Vol. 1** | S05 | Nice to Have | No | TBD / RenderX via Fab | Standard Fab License | **LOCKED** (Clutter/Charts) |
| **Farm Animals Sound Effects** | S02, S04 | Required | Loop | [$9.99 / AD Sounds](https://assetstore.unity.com/packages/audio/sound-fx/animals/farm-animals-sound-effects-269147) | Standard Unity | **LOCKED** |
| **HQ Clear Daytime Skybox** | S02, S03 | Required | No | Free / Custom | Open Source | **LOCKED** |
| **Selection Highlight Shader** | S03, S06 | Required | Yes | Custom URP Shader / Script | Internal | Ready to Build |

---

## 🆕 REQUIRED: URS Insemination Procedure Assets (Fasa 1-4)
To build the 4-phase breeding pipeline, we need you to find and source the following rugged, cleanable, and agricultural-veterinary themed 3D models.

### Fasa 1: Penyediaan Bahan Baka (Thawing Prep)
1. **Forceps (Pinset Semen)**
   - *Description:* Long metal forceps used to pick the semen straw safely from the liquid nitrogen.
   - *Target:* Low poly model, clean grabbable handle.
2. **Field Storage Nitrogen Tank (Tangki Simpanan Lapangan)**
   - *Description:* Cryogenic flask used for storing semen straws at -196°C.
   - *Target:* Rugged metal canister with a removable cap.
3. **Warm Water Beaker / Thawing Container (Bekas Air Hangat)**
   - *Description:* Small container or thermos bottle used for water bath thawing (35°C–37°C).
   - *Target:* Needs an open top so the player can drop the straw inside.
4. **Straw Cutter (Pemotong Straw)**
   - *Description:* Small specialized hand tool to snip the sealed end of the semen straw.
   - *Target:* Simple shear/cutter model.

### Fasa 2: Pemuatan Radas (Equipment Assembly)
5. **Insemination Gun / Pistolet (Alat Pistolet)**
   - *Description:* Long stainless-steel syringe gun used to deliver semen.
   - *Target:* Slender rod with a plunger mechanism.
6. **AI Sheath / Protective Sleeve (Selongsong Pelindung)**
   - *Description:* Disposable plastic sheath that fits over the loaded pistolet.
   - *Target:* Very simple translucent or white plastic tube mesh.
7. **PD Glove (Sarung Tangan Plastik Panjang)**
   - *Description:* Long shoulder-length plastic glove worn by the operator.
   - *Target:* Can be a texture overlay, or a simple hand-arm glove mesh overlay for the left virtual controller.

### Fasa 3 & 4: Insertion & Deposition Visual Aids
8. **Anatomy Cross-Section Panel (Paparan Panduan Anatomi)**
   - *Description:* Vector/graphic diagram showing the side profile of a cow's reproductive tract (vulva, vagina, cervical rings, uterine horns).
   - *Target:* Transparent PNG texture/sprite to place on a virtual clipboard/monitor screen.
9. **Semen Straw (Straw Semen)**
   - *Description:* Thin plastic straw containing the semen.
   - *Target:* Small colored cylinder with plug markings.

---

## Production Naming Conventions
Enforce strict snake_case naming keys prefixed by asset type:
- **Prefabs:** `pref_` (e.g. `pref_semen_forceps`, `pref_nitrogen_tank`)
- **Static Meshes:** `sm_` (e.g. `sm_straw_cutter`, `sm_water_beaker`)
- **Materials:** `mat_` (e.g. `mat_stainless_steel`, `mat_plastic_sheath`)
- **Textures:** `tex_` (e.g. `tex_anatomy_diagram_color`)
- **Audio Clips:** `snd_` (e.g. `snd_straw_cut_click`, `snd_water_splash`)

---

## Staging & Optimization Workflow
Before dropping imported geometry directly into your scene layout:
1. **Unpack and Re-Prefab:** Drag your environment components into a staging scene, strip vendor scripts, and configure them as Variant Prefabs.
2. **Materials Optimization:** Combine materials where possible or use simple mobile URP Lit/Unlit shaders to keep draw calls low on Quest standalone.
3. **VCS .keep Files:** If you are committing this directory layout before raw assets are completely assigned, place an empty `.keep` file inside empty folders.
