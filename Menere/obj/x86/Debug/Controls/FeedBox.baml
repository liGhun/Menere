   M S B A M L   `   `   `  ����
  Menere    Menere.Controls.FeedBox   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5        � 	Resources  5         FadeAway g�Name �� 
TargetName �� TargetProperty W� From	 W� To g� Duration g� AutoReverse g� RepeatBehavior 
 FadeIn��%�  &	        &	 �     �� 5	   
   .��5
      W� $ DimmAway��6   $ wrapPanelAvatarOverlay��5   -   $ 	(Opacity)�5   -   $
 0.75X�5   -   $	 0.0X�69   $ 0:0:1D�6B    .  $ 1x��5   A   5      �� 5   
   .��5      W� $ DimmIn��6   $ wrapPanelAvatarOverlay��5   -   $ 	(Opacity)�5   -   $
 0.75X�5   -   $
 0.75X�69   $ 0:0:1D�6C    .  $ 1x��5   A   5      5      .��5      � $��Stretch=�6   .+�5   
   �� 
�������$��1,1,10,1q�6(   	 �� CornerRadius$	 5��6C   
�������$��0,0,0,4q�6g   
 �� Padding$
 3,3,3,3q�6x   $��Stretch=�6�   ._�5      � -   5        $��GridMainItem��5   �   $	��Top=�6   $��Stretch=�6f    � ColumnDefinitions 5      �� 5       $��52 �6'   �� 5!      $��* �6'   .+�5#      � $��0��6   $	��Top=�6'   $
��Left=�6?   .+�5$      �� -   5        $��imageFeedIcon��5$   �   $
��Left=�6   $	��Top=�67   $��48��6P   $��48��6[   $��0,0,0,0q�6g   $��64��6x   $��64��6�   ��6�   ��  �� Path$
 Icon�� 5&      -   5        $��wrapPanelAvatarOverlay��5&   T   $
��Left=�6    $	��Top=�6;   $��48��6r   $��48��6}   
���   � J� Opacity$ 0Q�6�   $��0,0,0,0q�6�   .+�5'      � $��48��6    $��0,0,0,0q�6+   $��48��6<   $
��Left=�6H   $	��Top=�6c   5)      5+      5,      � 5-      $��1��6   $��5,0,0,0q�6'    � RowDefinitions 5.      �� 5/      �� 50      �� 51      .+�53      � $��0��6    54      �� 55      $��* �6/   �� 56      $��14 �6/   .+�58      � -   5        $��wrapPanelForTitle��58   $   $��0��6=    � Orientation$ 
Horizontal=�6M   .+�59      �� -   5        $��textblockTitle��59   (   $
��Bold"�6Z   $
��Wrap=�6l   ��6>   �� $ title�6l   5:      5<      �� 5=      -   5         ��Name$" textblockNumberOfUnreadItems��5=   3   
���   �$��1��6Y   $
��Wrap=�6f   ��6z   �� $ UnreadItemsCount�5?      � 5A      -   5        $��AdditionalInfos��5A       $��2��67   .+�5B      �� 
���   �$
��Wrap=�6V   ��67   �� $ SubTitle�6V   5E      5F      5G      5H      5I      5J      