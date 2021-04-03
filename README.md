# UnityExperiments
This repo contains several experiments in Unity that I've made over the past four years.

**Cube Planet** (2019) was my first attempt at a procedurally generated voxel world. Everything of interest is in the Improvements scene. The camera code was a little broken and performance wasn't very optimized whatsoever, but I was able to make:
- Chunk-based 3D block rendering with loading and unloading (similar to what games such as Minecraft use)
- Low-quality LOD mesh that loads/unloads each portion of itself depending on distance from the camera
- Procedural biome generation that uses cellular automata to make each biome unique
- World seed is a variable called "mseed" that can be changed in the GenerateBiomes() method in Scripts/TerrainGen.cs
- Quite glitchy camera controls - use WASD to move around, and hold right-click to rotate the camera.
Bear in mind that the skybox (or "starbox," I guess) is not mine. I think I searched up nebula skyboxes on Google Images or something as just a temporary thing.
Additionally, there's an older version available with some differences:
- No multithreading, so you've kinda got to wait for the full planet to load before you can do anything at a reasonable framerate. There's a percentage in the Unity console to keep track of how much of the planet has loaded.
- A significantly buggier voxel-based LOD system (as compared to the mesh one in the final version).
- Less finished terrain generation that had some strange artifacts.
- A bit less buggy camera controls.
 
**Space Game** (2019) has two scenes of interest.
ProcSystem:
- Has no camera control but you can see stuff being made in the Scene view.
- Custom Star shader, unlike the texture workaround used in Cube Planet.
- Procedural chunked planet mesh building using Marching Cubes. (Marching Cubes algorithm wasn't *entirely* written by me but I adapted it to Unity and modified it to work with multiple planet chunks.)
- Skybox is static, but was made from an ingame screenshot of the ProcGalaxy scene.
ProcGalaxy:
- IMPORTANT: Make sure you run this on your first monitor if you have a multi-monitor setup! The camera control system is heavily unfinished and glitchy.
- Camera can only be controlled on toggle - right click to toggle whether you can move it or not.
- Movement uses WASD with Q and E allowing you to move vertically.
- Camera can be rotated with the mouse.
- Features dynamic galaxy generation, with an infinite 3D grid of stars split into different sectors.
- Supports multiple sector types, including sparse stars, dense stars, and globular clusters.
- Dynamic shader-based nebula generation throughout the galaxy.
- Galaxy Sector dropdown in Universe Master was originally used to change the colors of the nebulae. It doesn't do anything anymore.
- If you'd like to run this as a standalone app outside of the editor, check Builds/ProjectExonaut_GalaxyMapBuild1.


**Exonaut** (2020) has a few different experiments.
ShaderTest:
- A small experiment with cel shading.
- I was originally going to use this as the full game's art style but I never actually put all the different systems together.
TerrainTest:
- Another attempt at smooth planet generation, this time using the Surface Nets algorithm instead of Marching Cubes. All the code here was written entirely by me.
- Has colored terrain!
- Has some rudimentary cave generation.
- At the time I stopped working on this I was trying to implement a smooth LOD system.
ControlTest:
- The last thing I really did with this project.
- This was an attempt at making my own character controller. It didn't go super well since I never really finished it.
- You can walk around and whatnot with WASD. There's an issue where sometimes you'll fall over - again, this was quite unfinished.


Additionally, this repo contains two small demos from 2017 when I was first learning Unity:
- **Blocky RPG** is a 2D project with parallax and an animated bird sprite.
- **RPG 3D** was a super small first attempt at learning 3D stuff. Has an incredibly buggy character controller, changing perspectives, and an environment comprised of free textures I found on the internet.
