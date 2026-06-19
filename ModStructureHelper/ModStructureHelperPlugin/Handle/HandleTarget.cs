using System.Collections.Generic;
using UnityEngine;

namespace ModStructureHelperPlugin.Handle;

public class HandleTarget
{
    private readonly IReadOnlyList<Transform> _targets;

    private Vector3 _startPivotPosition;
    private Quaternion _startPivotRotation;
    private Vector3 _startPivotScale;

    public Vector3 PivotPosition { get; private set; }
    public Quaternion PivotRotation { get; private set; }
    public Vector3 Scale { get; private set; }

    private readonly InitialManipulationState[] _initialStates;
    public bool Manipulating { get; private set; }
    
    public HandleTarget(Transform target)
    {
        _targets = new [] { target };
        
        _initialStates = new InitialManipulationState[_targets.Count];
        
        PivotPosition = target.position;
        PivotRotation = target.rotation;
        Scale = target.lossyScale;
    }
    
    public HandleTarget(IReadOnlyList<Transform> targets)
    {
        _targets = targets;
        
        _initialStates = new InitialManipulationState[_targets.Count];
        
        CalculatePivotForSelection();
    }

    private void CalculatePivotForSelection()
    {
        PivotPosition = CalculatePivotCenter();
        PivotRotation = CalculatePivotRotation();
        Scale = Vector3.one;
    }

    private Vector3 CalculatePivotCenter()
    {
        int validTargets = 0;
        Vector3 sum = default;
        foreach (var target in GetTargetTransforms(true))
        {
            validTargets++;
            sum += target.position;
        }

        if (validTargets == 0)
            return default;
        
        return sum / validTargets;
    }

    // Returns the pivot of the last selected object
    private Quaternion CalculatePivotRotation()
    {
        Quaternion targetRotation = Quaternion.identity;
        for (int i = 0; i < _targets.Count; i++)
        {
            if (_targets[i] == null) continue;
            targetRotation = _targets[i].rotation;
        }

        return targetRotation;
    }

    public void StartManipulation()
    {
        if (Manipulating)
        {
            Plugin.Logger.LogWarning("Starting manipulation on an object while already manipulating it!");
        }
        
        Manipulating = true;
        
        CalculatePivotForSelection();
        
        _startPivotPosition = PivotPosition;
        _startPivotRotation = PivotRotation;
        _startPivotScale = Scale;
        
        for (int i = 0; i < _targets.Count; i++)
        {
            if (_targets[i] == null)
            {
                Plugin.Logger.LogWarning($"Target at index {i} is null!");
                continue;
            }
            _initialStates[i].Position = _targets[i].position;
            _initialStates[i].Rotation = _targets[i].rotation;
            _initialStates[i].Scale = _targets[i].localScale;
        }
    }

    public void EndManipulation()
    {
        CalculatePivotForSelection();
        Manipulating = false;
    }

    public void Refresh()
    {
        EndManipulation();
        StartManipulation();
        EndManipulation();
    }

    public void SetPivotPosition(Vector3 newPosition)
    {
        PrepareMove();
        var delta = newPosition - _startPivotPosition;
        
        for (int i = 0; i < _targets.Count; i++)
        {
            if (_targets[i] == null) continue;
            _targets[i].position = _initialStates[i].Position + delta;
        }

        PivotPosition = newPosition;
    }

    public void SetPivotRotation(Quaternion newRotation)
    {
        PrepareMove();
        Quaternion totalDelta =
            newRotation *
            Quaternion.Inverse(_startPivotRotation);
        
        for (int i = 0; i < _targets.Count; i++)
        {
            if (_targets[i] == null) continue;
            Vector3 offset =
                _initialStates[i].Position - _startPivotPosition;

            _targets[i].position =
                PivotPosition + totalDelta * offset;

            _targets[i].rotation =
                totalDelta * _initialStates[i].Rotation;
        }

        PivotRotation = newRotation;
    }

    public void SetScale(Vector3 newScale)
    {
        PrepareMove();
        Vector3 scaleMultiplier = Vector3.zero;
        if (!Mathf.Approximately(_startPivotScale.x, 0f)
            && !Mathf.Approximately(_startPivotScale.y, 0f)
            && !Mathf.Approximately(_startPivotScale.z, 0f))
        {
            scaleMultiplier = new Vector3(newScale.x / _startPivotScale.x, newScale.y / _startPivotScale.y, newScale.z / _startPivotScale.z);
        }
        else
        {
            Plugin.Logger.LogWarning("Warning: transform has scale of 0! Avoiding divide by zero");
        }
        
        for (int i = 0; i < _targets.Count; i++)
        {
            if (_targets[i] == null) continue;
            var positionFromCenter = _initialStates[i].Position - _startPivotPosition;
            _targets[i].position = _startPivotPosition + Vector3.Scale(positionFromCenter, scaleMultiplier);
            _targets[i].localScale = Vector3.Scale(_initialStates[i].Scale, scaleMultiplier);
        }
        
        Scale = newScale;
    }

    private void PrepareMove()
    {
        if (!Manipulating)
        {
            Plugin.Logger.LogWarning("Attempting to move handle target while manipulation is inactive!");
            StartManipulation();
        }
    }

    public IEnumerable<Transform> GetTargetTransforms(bool skipInvalid)
    {
        foreach (var target in _targets)
        {
            if (!skipInvalid || target != null)
                yield return target;
        }
    }

    private struct InitialManipulationState
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;
    }
}