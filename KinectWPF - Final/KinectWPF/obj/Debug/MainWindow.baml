   M S B A M L   `   `   `  ����  	KinectWPF    KinectWPF.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  Form Analysis��5   	   $
��1080��6   $
��1920��6-   .��5      � .+�5   
   �� -   5        $��camera��5      $��
10,0,10,10q�6   �� 5   
   -   5        $��canvas��5      $��
10,0,10,10q�6   � 5   
    �� Left$ 72��6   
 �� Top$	 609��6    $
��1734��61   $��89,58,89,85q�6>    � ColumnDefinitions 5	      �� 5
      $
��647* �6#   �� 5      $��1087* �6#   .+�5      �� -   5         ��Name$ btnAnalysis��5      $��Start Analysis��6*   $
��Left=�6C   $��806,715,0,0q�6^   $	��Top=�6s   $	��167��6�   $��82��6�   $��18(�6�   $��1��6�   ���	#FF07A81D
�������6�   �� 5      -   5         ��Name$ imgHandDirection��5      $��Images/InfoIcon.png��6.   $
��Left=�6K   $��36��6f   $��
73,691,0,0q�6r   $	��Top=�6�   $��36��6�   6�   �� 5      -   5         ��Name$ grpBoxHandPref��5      $��Hand Preference��6/   $	��150��6H   $	��420��6U   $��0q�6a   $��18(�6m   
�������$��114,691,113,65q�6�   $��0q�6�   .��5      � $��-13,-6,-6,19q�6    � RowDefinitions 5      �� 5      $��6* �6(   �� 5      $
��101* �6(   .+�5      �� -   5        $ btnLeftHand��5      $
��Left��62   $��27,4,232,24q�6W   ���	#FFCDD606$��1��6�   �� 5      -   5        $ btnRightHand��5      $��Right��63   $��227,4,32,24q�6^   ���	#FFCDD606$��1��6�   5      5      �� 5      -	   5        $ grpBoxJointButtons��5      $��Joint Colour��63   $��18(�6I   
�������$��764,10,-146,689q�6j   $��0q�6�   $��1��6�   .��5      � -
   5         �Name$ grdJointButtons��5      $��0,-43,9,-74q�60    5      �� 5      $
��709* �6+   �� 5      $	��85* �6+   .+�5       �� -   5        	 ��Name$	 cbJointColour��5       $��28,48,184,182q�66   ��6M   �� *	Colours+6�   5!      5"      �� 5#      -   5        $ imgJointColour��5#      $��Images/InfoIcon.PNG��6,   $
��Left=�6I   $��36��6d   $��
728,10,0,0q�6p   $	��Top=�6�   $��36��6�   $��1��6�   6  �� 5$      -   5        $ imgAnalysis��5$      $��Images/InfoIcon.PNG��6)   $
��Left=�6F   $��36��6a   $��750,691,0,0q�6m   $	��Top=�6�   $��36��6�   $��1��6�   6  5'      �� 5(   
   -   5        
 ��Name$
 txtInfo��5(      $��2��6#   $
��Left=�67   $��
581,53,0,0q�6R    �� TextWrapping$
 Wrap=�6f   $��TextBox��6z   $	��Top=�6�   $	��616��6�    �� 	IsEnabled . $��48(�6�    �� TextAlignment$ Center=�6�    �� 
IsReadOnly . $y�Hidden=�6  ��6�   Z� 6  �� 5)   
   -   5        $
 txtControlInfo��5)      $
��Left=�6*   $	��121��6E   $��631,609,0,0q�6R   $
 Wrap=�6g   $��TextBox��6{   $	��Top=�6�   $	��616��6�    . $��48(�6�   $ Center=�6�    . $y�Hidden=�6  ��6�   Z� 6  5+      5,      