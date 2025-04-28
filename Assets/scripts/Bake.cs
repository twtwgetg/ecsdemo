using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class Bake : MonoBehaviour
{
 
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public Animator animator;
    public AnimationClip[] animationClips;

    private List<Mesh> bakedMeshes = new List<Mesh>();
    
    void Start()
    {
        // 获取SkinnedMeshRenderer组件
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        // 获取Animator组件
        animator = GetComponent<Animator>();
        // 获取所有动画片段
        animationClips = animator.runtimeAnimatorController.animationClips;

    }
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.D))
        //{

        //    // 遍历每个动画片段进行烘焙
        //    foreach (AnimationClip clip in animationClips)
        //    {
        //        BakeAnimation(clip);
        //    }
        //}
    }

    void BakeAnimation(AnimationClip clip)
    {
        // 获取动画的帧率
        int frameRate = Mathf.RoundToInt(clip.frameRate);
        // 计算动画的总帧数
        int totalFrames = Mathf.RoundToInt(clip.length * frameRate);
        for (int i = 0; i <= totalFrames; i++)
        {
            float time = i / (float)frameRate;
            clip.SampleAnimation(gameObject, time);
            Mesh bakedMesh = new Mesh();
            // 烘焙当前帧的SkinnedMesh
            skinnedMeshRenderer.BakeMesh(bakedMesh);
#if UNITY_EDITOR
            AssetDatabase.CreateAsset(bakedMesh, "Assets/BakedMeshes/" + clip.name + "_" + i + ".asset");
#endif
        }
        //// 遍历每一帧进行烘焙
        //for (int frame = 0; frame < totalFrames; frame++)
        //{
        //    // 设置当前帧的时间
        //    animator.Update(1f / frameRate);

        //    // 创建一个新的Mesh来存储烘焙结果
        //    Mesh bakedMesh = new Mesh();
        //    // 烘焙当前帧的SkinnedMesh
        //    skinnedMeshRenderer.BakeMesh(bakedMesh);


        //    // 将烘焙的Mesh添加到列表中
        //    bakedMeshes.Add(bakedMesh);
        //}

        // 停止播放动画
        //animator.stop.Stop();
    } 
    public void tBake()
    {
        //animationClips = animator.runtimeAnimatorController.animationClips;
        for(int i= 0; i < animationClips.Length; i++){
            var x = animationClips[i];
            //if (x.name == "Warrior_Attack")
            {
                BakeAnimation(x);
            }
        }
    }
}
