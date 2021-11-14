# Overview

Given an RGBD (Red/gree/blue/depth) image, we wish to synthesize a pair of left/right stereo images.

This repository will show how to perform this task in Unity3D using C# and HLSL (shader language).

# How is this technique different from other view synthesis techniques?

This technique will 
- be easier to set up in Unity3D
- not rely upon artificial intelligence
- be targeted to run in real time

# Source data

Source of RGBD dataset is https://cs.nyu.edu/~silberman/datasets/nyu_depth_v2.html

Publication: Indoor Segmentation and Support Inference from RGBD Images ECCV 2012
- Authors: Nathan Silberman, Pushmeet Kohli, Derek Hoiem, Rob Fergus
- http://cs.nyu.edu/~silberman/papers/indoor_seg_support.pdf
- http://cs.nyu.edu/~silberman/bib/indoor_seg_support.bib

