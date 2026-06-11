using System.Collections;
using System.Collections.Generic;
using Games.Reefscape.Enums;
using Games.Reefscape.GamePieceSystem;
using Games.Reefscape.Robots;
using RobotFramework.Components;
using RobotFramework.Controllers.GamePieceSystem;
using RobotFramework.Controllers.PidSystems;
using RobotFramework.Enums;
using RobotFramework.GamePieceSystem;
using UnityEngine;

namespace Prefabs.Reefscape.Robots.Mods.StemColiseumMod._172
{
    public class NorthernForce: ReefscapeRobotBase
    {
        [SerializeField] private GenericElevator elevator;
        [SerializeField] private GenericJoint algaeArm;
        
        [SerializeField] private PidConstants algaeArmPid;

        [SerializeField] private ReefscapeGamePieceIntake coralIntake;
        [SerializeField] private ReefscapeGamePieceIntake algaeIntake;

        [SerializeField] private GamePieceState coralStowState;
        [SerializeField] private GamePieceState algaeStowState;

        private RobotGamePieceController<ReefscapeGamePiece, ReefscapeGamePieceData>.GamePieceControllerNode _coralController;
        private RobotGamePieceController<ReefscapeGamePiece, ReefscapeGamePieceData>.GamePieceControllerNode _algaeController;

        [SerializeField] private NorthernForceSetpoint stow;
        [SerializeField] private NorthernForceSetpoint intake;
        [SerializeField] private NorthernForceSetpoint intakeAlgae;
        [SerializeField] private NorthernForceSetpoint l1;
        [SerializeField] private NorthernForceSetpoint l2;
        [SerializeField] private NorthernForceSetpoint l3;
        [SerializeField] private NorthernForceSetpoint l4;
        [SerializeField] private NorthernForceSetpoint l4Place;
        [SerializeField] private NorthernForceSetpoint lowAlgae;
        [SerializeField] private NorthernForceSetpoint highAlgae;
        [SerializeField] private NorthernForceSetpoint processor;
        [SerializeField] private NorthernForceSetpoint bargePrep;
        [SerializeField] private NorthernForceSetpoint bargePlace;

        private float _elevatorTargetHeight;
        private float _algaeArmTargetAngle;

        protected override void Start()
        {
            base.Start();

            algaeArm.SetPid(algaeArmPid);

            _elevatorTargetHeight = 0;
            _algaeArmTargetAngle = -90;

            RobotGamePieceController.SetPreload(coralStowState);
            _coralController = RobotGamePieceController.GetPieceByName(ReefscapeGamePieceType.Coral.ToString());
            _algaeController = RobotGamePieceController.GetPieceByName(ReefscapeGamePieceType.Algae.ToString());

            _coralController.gamePieceStates = new[]
            {
                coralStowState
            };
            _coralController.intakes.Add(coralIntake);

            _algaeController.gamePieceStates = new[] {algaeStowState };
            _algaeController.intakes.Add(algaeIntake);
        }

        private void SetSetpoint(NorthernForceSetpoint setpoint)
        {
            _elevatorTargetHeight = setpoint.elevatorHeight;
            _algaeArmTargetAngle = setpoint.algaeArmAngle;
        }

        private void UpdateSetpoints()
        {
            elevator.SetTarget(_elevatorTargetHeight);
            algaeArm.SetTargetAngle(_algaeArmTargetAngle).withAxis(JointAxis.X);
        }

        private void PlacePiece()
        {
            if (_coralController.HasPiece())
            {
                if (LastSetpoint == ReefscapeSetpoints.L4)
                {
                    _coralController.ReleaseGamePieceWithContinuedForce(new Vector3(0, 0, 8), 0.5f, 0.5f);
                }
                else
                {
                    _coralController.ReleaseGamePieceWithForce(new Vector3(0, 0, 5));
                }
            }
            else if (LastSetpoint == ReefscapeSetpoints.Barge || LastSetpoint == ReefscapeSetpoints.Processor)
            {
                _algaeController.ReleaseGamePieceWithForce(new Vector3(0, 0, 5));
            }
        }

        private void LateUpdate()
        {
            algaeArm.UpdatePid(algaeArmPid);
        }

        private void FixedUpdate()
        {
            bool hasCoral = _coralController.HasPiece();
            bool hasAlgae = _algaeController.HasPiece();

            _coralController.SetTargetState(coralStowState);
            _algaeController.SetTargetState(algaeStowState);

            switch (CurrentSetpoint)
            {
                case ReefscapeSetpoints.Stow:
                    SetSetpoint(stow);
                    break;
                case ReefscapeSetpoints.Intake:
                    SetSetpoint(intake);
                    _coralController.RequestIntake(coralIntake, CurrentRobotMode == ReefscapeRobotMode.Coral && !hasCoral);
                    _algaeController.RequestIntake(algaeIntake, CurrentRobotMode == ReefscapeRobotMode.Algae && !hasAlgae);
                    break;
                case ReefscapeSetpoints.Place:
                    if (LastSetpoint == ReefscapeSetpoints.Barge)
                    {
                        SetSetpoint(bargePlace);
                    }
                    else if (LastSetpoint == ReefscapeSetpoints.L4)
                    {
                        SetSetpoint(l4Place);
                    }
                    PlacePiece();
                    break;
                case ReefscapeSetpoints.L1:
                    SetSetpoint(l1);
                    break;
                case ReefscapeSetpoints.Stack:
                    SetSetpoint(intakeAlgae);
                    _coralController.RequestIntake(coralIntake, false);
                    _algaeController.RequestIntake(algaeIntake, IntakeAction.IsPressed() && !hasAlgae);
                    break;
                case ReefscapeSetpoints.L2:
                    SetSetpoint(l2);
                    break;
                case ReefscapeSetpoints.LowAlgae:
                    SetSetpoint(lowAlgae);
                    _coralController.RequestIntake(coralIntake, false);
                    _algaeController.RequestIntake(algaeIntake, IntakeAction.IsPressed() && !hasAlgae);
                    break;
                case ReefscapeSetpoints.L3:
                    SetSetpoint(l3);
                    break;
                case ReefscapeSetpoints.HighAlgae:
                    SetSetpoint(highAlgae);
                    _coralController.RequestIntake(coralIntake, false);
                    _algaeController.RequestIntake(algaeIntake, IntakeAction.IsPressed() && !hasAlgae);
                    break;
                case ReefscapeSetpoints.L4:
                    SetSetpoint(l4);
                    break;
                case ReefscapeSetpoints.Processor:
                    SetSetpoint(processor);
                    break;
                case ReefscapeSetpoints.Barge:
                    SetSetpoint(bargePrep);
                    break;
                case ReefscapeSetpoints.RobotSpecial:
                    SetState(ReefscapeSetpoints.Intake);
                    break;
                case ReefscapeSetpoints.Climb:
                    break;
                case ReefscapeSetpoints.Climbed:
                    break;
            }
            
            UpdateSetpoints();
        }
    }
}