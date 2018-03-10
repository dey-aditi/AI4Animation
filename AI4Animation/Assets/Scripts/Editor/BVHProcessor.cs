﻿using UnityEngine;
using UnityEditor;
using System.IO;

public class BVHProcessor : EditorWindow {

	public static EditorWindow Window;
	public static Vector2 Scroll;

	public string Directory = string.Empty;
	public int Framerate = 60;
	public bool[] Use = new bool[0];
	public BVHAnimation[] Animations = new BVHAnimation[0];

	private static string Separator = " ";
	private static string Accuracy = "F6";

	[MenuItem ("Addons/BVH Processor")]
	static void Init() {
		Window = EditorWindow.GetWindow(typeof(BVHProcessor));
		Scroll = Vector3.zero;
	}

	void OnGUI() {
		Utility.SetGUIColor(UltiDraw.Black);
		using(new EditorGUILayout.VerticalScope ("Box")) {
			Utility.ResetGUIColor();

			Utility.SetGUIColor(UltiDraw.Grey);
			using(new EditorGUILayout.VerticalScope ("Box")) {
				Utility.ResetGUIColor();

				Utility.SetGUIColor(UltiDraw.Orange);
				using(new EditorGUILayout.VerticalScope ("Box")) {
					Utility.ResetGUIColor();
					EditorGUILayout.LabelField("Processor");
				}

				if(Utility.GUIButton("Export Labels", UltiDraw.DarkGrey, UltiDraw.White)) {
					ExportLabels();
				}
				if(Utility.GUIButton("Export Data", UltiDraw.DarkGrey, UltiDraw.White)) {
					ExportData();
				}
				if(Utility.GUIButton("Data Distribution", UltiDraw.DarkGrey, UltiDraw.White)) {
					PrintDataDistribution();
				}

				EditorGUILayout.BeginHorizontal();
				if(Utility.GUIButton("Enable All", UltiDraw.Grey, UltiDraw.White)) {
					for(int i=0; i<Use.Length; i++) {
						Use[i] = true;
					}
				}
				if(Utility.GUIButton("Disable All", UltiDraw.Grey, UltiDraw.White)) {
					for(int i=0; i<Use.Length; i++) {
						Use[i] = false;
					}
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.LabelField("Export Time: " + GetExportTime() + "s");
				
                if(Utility.GUIButton("Fix Data", UltiDraw.DarkGreen, UltiDraw.White)) {
                    for(int i=0; i<Animations.Length; i++) {
						BVHAnimation animation = Animations[i];
                        EditorUtility.SetDirty(Animations[i]);
                    }
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }

				Framerate = EditorGUILayout.IntField("Framerate", Framerate);				
				
				Scroll = EditorGUILayout.BeginScrollView(Scroll);
				using(new EditorGUILayout.VerticalScope ("Box")) {
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Assets/", GUILayout.Width(45f));
					LoadDirectory(EditorGUILayout.TextField(Directory));
					EditorGUILayout.EndHorizontal();

					for(int i=0; i<Animations.Length; i++) {
						if(Use[i]) {
							Utility.SetGUIColor(UltiDraw.DarkGreen);
						} else {
							Utility.SetGUIColor(UltiDraw.DarkRed);
						}
						using(new EditorGUILayout.VerticalScope ("Box")) {
							Utility.ResetGUIColor();
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField((i+1).ToString(), GUILayout.Width(20f));
							Use[i] = EditorGUILayout.Toggle(Use[i], GUILayout.Width(20f));
							Animations[i] = (BVHAnimation)EditorGUILayout.ObjectField(Animations[i], typeof(BVHAnimation), true);
							EditorGUILayout.EndHorizontal();
						}
					}
					
				}

				EditorGUILayout.EndScrollView();
			}
		}
	}

	private void LoadDirectory(string dir) {
		if(Directory != dir) {
			Directory = dir;
			Animations = new BVHAnimation[0];
			Use = new bool[0];
			string path = "Assets/"+Directory;
			if(AssetDatabase.IsValidFolder(path)) {
				string[] elements = AssetDatabase.FindAssets("t:BVHAnimation", new string[1]{path});
				Animations = new BVHAnimation[elements.Length];
				Use = new bool[elements.Length];
				for(int i=0; i<elements.Length; i++) {
					Animations[i] = (BVHAnimation)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(elements[i]), typeof(BVHAnimation));
					Use[i] = true;
				}
			}
		}
	}

	private void ExportLabels() {
		if(Animations.Length == 0) {
			Debug.Log("No animations specified.");
			return;
		}
		
		string name = "Labels";
		string filename = string.Empty;
		if(!File.Exists(Application.dataPath+"/Project/"+name+".txt")) {
			filename = Application.dataPath+"/Project/"+name;
		} else {
			int i = 1;
			while(File.Exists(Application.dataPath+"/Project/"+name+" ("+i+").txt")) {
				i += 1;
			}
			filename = Application.dataPath+"/Project/"+name+" ("+i+")";
		}

		StreamWriter labels = File.CreateText(filename+".txt");
		int index = 0;
		labels.WriteLine(index + " " + "Sequence"); index += 1;
		labels.WriteLine(index + " " + "Frame"); index += 1;
		labels.WriteLine(index + " " + "Timestamp"); index += 1;
		for(int i=0; i<Animations[0].Character.Hierarchy.Length; i++) {
			if(Animations[0].Bones[i]) {
				labels.WriteLine(index + " " + Animations[0].Character.Hierarchy[i].GetName() + "PositionX"+(i+1)); index += 1;
				labels.WriteLine(index + " " + Animations[0].Character.Hierarchy[i].GetName() + "PositionY"+(i+1)); index += 1;
				labels.WriteLine(index + " " + Animations[0].Character.Hierarchy[i].GetName() + "PositionZ"+(i+1)); index += 1;
				labels.WriteLine(index + " " + Animations[0].Character.Hierarchy[i].GetName() + "ForwardX"+(i+1)); index += 1;
				labels.WriteLine(index + " " + Animations[0].Character.Hierarchy[i].GetName() + "ForwardY"+(i+1)); index += 1;
				labels.WriteLine(index + " " + Animations[0].Character.Hierarchy[i].GetName() + "ForwardZ"+(i+1)); index += 1;
				labels.WriteLine(index + " " + Animations[0].Character.Hierarchy[i].GetName() + "UpX"+(i+1)); index += 1;
				labels.WriteLine(index + " " + Animations[0].Character.Hierarchy[i].GetName() + "UpY"+(i+1)); index += 1;
				labels.WriteLine(index + " " + Animations[0].Character.Hierarchy[i].GetName() + "UpZ"+(i+1)); index += 1;
				labels.WriteLine(index + " " + Animations[0].Character.Hierarchy[i].GetName() + "VelocityX"+(i+1)); index += 1;
				labels.WriteLine(index + " " + Animations[0].Character.Hierarchy[i].GetName() + "VelocityY"+(i+1)); index += 1;
				labels.WriteLine(index + " " + Animations[0].Character.Hierarchy[i].GetName() + "VelocityZ"+(i+1)); index += 1;
			}
		}
		for(int i=1; i<=12; i++) {
			labels.WriteLine(index + " " + "TrajectoryPositionX"+i); index += 1;
			//labels.WriteLine(index + " " + "TrajectoryPositionY"+i); index += 1;
			labels.WriteLine(index + " " + "TrajectoryPositionZ"+i); index += 1;
			labels.WriteLine(index + " " + "TrajectoryVelocityX"+i); index += 1;
			//labels.WriteLine(index + " " + "TrajectoryVelocityY"+i); index += 1;
			labels.WriteLine(index + " " + "TrajectoryVelocityZ"+i); index += 1;
			labels.WriteLine(index + " " + "TrajectoryDirectionX"+i); index += 1;
			//labels.WriteLine(index + " " + "TrajectoryDirectionY"+i); index += 1;
			labels.WriteLine(index + " " + "TrajectoryDirectionZ"+i); index += 1;
			//labels.WriteLine(index + " " + "TrajectoryLeftHeight"+i); index += 1;
			//labels.WriteLine(index + " " + "TrajectoryRightHeight"+i); index += 1;
			for(int j=1; j<=Animations[0].StyleFunction.Styles.Length; j++) {
				labels.WriteLine(index + " " + Animations[0].StyleFunction.Styles[j-1].Name + i); index += 1;
			}
		}
		labels.WriteLine(index + " " + "TranslationalOffsetX"); index += 1;
		labels.WriteLine(index + " " + "TranslationalOffsetZ"); index += 1;
		labels.WriteLine(index + " " + "AngularOffsetY"); index += 1;

		//labels.WriteLine(index + " " + "Phase"); index += 1;
		//labels.WriteLine(index + " " + "PhaseUpdate");
		
		labels.Close();
	}

	private void ExportData() {
		if(Animations.Length == 0) {
			Debug.Log("No animations specified.");
			return;
		}
		
		string name = "Data";
		string filename = string.Empty;
		if(!File.Exists(Application.dataPath+"/Project/"+name+".txt")) {
			filename = Application.dataPath+"/Project/"+name;
		} else {
			int i = 1;
			while(File.Exists(Application.dataPath+"/Project/"+name+" ("+i+").txt")) {
				i += 1;
			}
			filename = Application.dataPath+"/Project/"+name+" ("+i+")";
		}

		StreamWriter data = File.CreateText(filename+".txt");
		int sequence = 0;
		WriteAnimations(ref data, ref sequence, false);
		WriteAnimations(ref data, ref sequence, true);
		data.Close();
	}

	private void WriteAnimations(ref StreamWriter data, ref int sequence, bool mirrored) {
		for(int i=0; i<Animations.Length; i++) {
			if(Use[i]) {
				for(int s=0; s<Animations[i].Sequences.Length; s++) {
					for(int e=0; e<Animations[i].Sequences[s].Export; e++) {
						sequence += 1;
						float timeStart = Animations[i].GetFrame(Animations[i].Sequences[s].Start).Timestamp;
						float timeEnd = Animations[i].GetFrame(Animations[i].Sequences[s].End).Timestamp;
						for(float j=timeStart; j<=timeEnd; j+=1f/(float)Framerate) {
							//Get frames
							BVHAnimation.BVHFrame frame = Animations[i].GetFrame(j);
							BVHAnimation.BVHFrame prevFrame = Animations[i].GetFrame(Mathf.Clamp(j-1f/(float)Framerate, 0f, Animations[i].GetTotalTime()));
							
							//Sequence number
							string line = sequence + Separator;

							//Frame index
							line += frame.Index + Separator;

							//Frame time
							line += frame.Timestamp + Separator;

							//Extract data
							Matrix4x4[] posture = Animations[i].ExtractPosture(frame, mirrored);
							//Vector3[] velocities = Animations[i].ExtractVelocities(frame, mirrored, 0.1f);
							Vector3[] boneVelocities = Animations[i].ExtractBoneVelocities(frame, mirrored);
							//for(int v=0; v<boneVelocities.Length; v++) {
							//	boneVelocities[v] /= Framerate;
							//}
							Trajectory currentTrajectory = Animations[i].ExtractTrajectory(frame, mirrored);
							Trajectory previousTrajectory = Animations[i].ExtractTrajectory(prevFrame, mirrored);
							
							//Get root transformation
							Matrix4x4 root = currentTrajectory.Points[6].GetTransformation();

							//Bone data
							for(int k=0; k<Animations[i].Character.Hierarchy.Length; k++) {
								if(Animations[i].Bones[k]) {
									//Position
									line += FormatVector3(posture[k].GetPosition().GetRelativePositionTo(root));

									//Rotation
									line += FormatVector3(posture[k].GetForward().GetRelativeDirectionTo(root));
									line += FormatVector3(posture[k].GetUp().GetRelativeDirectionTo(root));

									//Bone Velocity
									line += FormatVector3(boneVelocities[k].GetRelativeDirectionTo(root));
								}
							}
							
							//Trajectory data
							for(int k=0; k<12; k++) {
								Vector3 position = currentTrajectory.Points[k].GetPosition().GetRelativePositionTo(root);
								Vector3 facing = currentTrajectory.Points[k].GetDirection().GetRelativeDirectionTo(root);
								Vector3 velocity = currentTrajectory.Points[k].GetVelocity().GetRelativeDirectionTo(root);
								line += FormatValue(position.x);
								line += FormatValue(position.z);
								line += FormatValue(facing.x);
								line += FormatValue(facing.z);
								line += FormatValue(velocity.x);
								line += FormatValue(velocity.z);
								//line += FormatVector3(currentTrajectory.Points[k].GetPosition().GetRelativePositionTo(root));
								//line += FormatValue(Vector3.SignedAngle(root.GetForward(), currentTrajectory.Points[k].GetDirection(), Vector3.up));
								//line += FormatVector3(currentTrajectory.Points[k].GetVelocity().GetRelativeDirectionTo(root));
								//line += FormatValue(currentTrajectory.Points[k].GetDirection().GetRelativeDirectionTo(root).x);
								//line += FormatValue(currentTrajectory.Points[k].GetDirection().GetRelativeDirectionTo(root).z);
								//line += FormatValue(currentTrajectory.Points[k].GetLeftSample().y - root.GetPosition().y);
								//line += FormatValue(currentTrajectory.Points[k].GetRightSample().y - root.GetPosition().y);
								line += FormatArray(currentTrajectory.Points[k].Styles);
							}

							//Translational and angular root offset
							Matrix4x4 offset = currentTrajectory.Points[6].GetTransformation().GetRelativeTransformationTo(previousTrajectory.Points[6].GetTransformation());
							line += FormatValue(offset.GetPosition().x);
							line += FormatValue(offset.GetPosition().z);
							line += FormatValue(Vector3.SignedAngle(Vector3.forward, offset.GetForward(), Vector3.up));
							
							/*
							//Phase
							float prev = mirrored ? Animations[i].MirroredPhaseFunction.GetPhase(prevFrame) : Animations[i].PhaseFunction.GetPhase(prevFrame);
							float current = mirrored ? Animations[i].MirroredPhaseFunction.GetPhase(frame) : Animations[i].PhaseFunction.GetPhase(frame);
							line += FormatValue(current);
							line += FormatValue(GetPhaseUpdate(prev, current));
							*/

							//Postprocess
							line = line.Remove(line.Length-1);
							line = line.Replace(",",".");

							//Write
							data.WriteLine(line);
						}
					}
				}
			}
		}
	}

	private float GetExportTime() {
		float time = 0f;
		for(int i=0; i<Animations.Length; i++) {
			if(Use[i]) {
				for(int s=0; s<Animations[i].Sequences.Length; s++) {
					for(int e=0; e<Animations[i].Sequences[s].Export; e++) {
						time += Animations[i].Sequences[s].GetLength() * Animations[i].FrameTime;
					}
				}
			}
		}
		return time;
	}

	private void PrintDataDistribution() {
		if(Animations.Length == 0) {
			return;
		}
		string[] names = new string[Animations[0].StyleFunction.Styles.Length];
		for(int i=0; i<names.Length; i++) {
			names[i] = Animations[0].StyleFunction.Styles[i].Name;
		}
		int[] distribution = new int[Animations[0].StyleFunction.Styles.Length];
		int totalFrames = 0;
		for(int i=0; i<Animations.Length; i++) {
			if(Use[i]) {
				for(int s=0; s<Animations[i].Sequences.Length; s++) {
					for(int e=0; e<Animations[i].Sequences[s].Export; e++) {
						int startIndex = Animations[i].Sequences[s].Start-1;
						int endIndex = Animations[i].Sequences[s].End-1;
						for(int j=startIndex; j<=endIndex; j++) {
							totalFrames += 1;
							for(int d=0; d<distribution.Length; d++) {
								distribution[d] += Animations[i].StyleFunction.Styles[d].Flags[j] ? 1 : 0;
							}
						}
					}
				}
			}
		}
		
		Debug.Log("Total Frames: " + totalFrames);
		Debug.Log("Total Time: " + totalFrames * Animations[0].FrameTime);
		for(int i=0; i<names.Length; i++) {
			Debug.Log("Name: " + names[i] + " Frames: " + distribution[i] + " Time: " + distribution[i] * Animations[0].FrameTime + "s" + " Ratio: " + 100f*((float)distribution[i] / (float)totalFrames) + "%");
		}
	}

	private float GetPhaseUpdate(float prev, float next) {
		return Mathf.Repeat(((next-prev) + 1f), 1f);
	}

	private string FormatString(string value) {
		return value + Separator;
	}

	private string FormatValue(float value) {
		return value.ToString(Accuracy) + Separator;
	}

	private string FormatArray(float[] array) {
		string format = string.Empty;
		for(int i=0; i<array.Length; i++) {
			format += array[i].ToString(Accuracy) + Separator;
		}
		return format;
	}

	private string FormatArray(bool[] array) {
		string format = string.Empty;
		for(int i=0; i<array.Length; i++) {
			float value = array[i] ? 1f : 0f;
			format += value.ToString(Accuracy) + Separator;
		}
		return format;
	}

	private string FormatVector3(Vector3 vector) {
		return vector.x.ToString(Accuracy) + Separator + vector.y.ToString(Accuracy) + Separator + vector.z.ToString(Accuracy) + Separator;
	}

	private string FormatQuaternion(Quaternion quaternion, bool imaginary, bool real) {
		string output = string.Empty;
		if(imaginary) {
			output += quaternion.x + Separator + quaternion.y + Separator + quaternion.z + Separator;
		}
		if(real) {
			output += quaternion.w + Separator;
		}
		return output;
	}

}