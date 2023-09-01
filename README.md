# YuTools
## 目前包含的功能：
  1.UI组件自动绑定。
  2.资源依赖查找。
  3.预制体自动检测RaycastTarget开关。
## UI组件自动绑定
  主要就是通过创建组件时的命名规范跟预设文本模板来生成.cs。
### 主要脚本：
  1.UIViewBind.cs：主要绑定相关的方法。
  2.YuToolsSetting.cs：绑定相关设置，如模板，txt模板路径，绑定赋值语句等。
  3.UIBindWindow.cs：打开的绑定GUI窗口。
  4.UIBindTemplete.txt：.cs模板。
### 使用方法：
  1.将YuTools放入Editor文件夹下。
  2.需要将YuToolsSetting.cs和UIBindTemplete.txt两个文件的规范、规则相关等修改成符合所在项目。
  3.确定修改无误后：
    - 右键选中View预制体或者直接在Project窗口右键，快捷键Alt+Shift+Y
    - 打开绑定窗口，放入相应的文件或文件夹路径(可手动写入不存在的文件夹，自动创建)
    - 成功创建后在指定路径查看生成的.cs
### 注意：
  记得改设置，改模板，本库中的设置与模板并不是通用的。

## 资源依赖查找
  基本上就是遍历选中的资源，获取他们的依赖，大把开源。
  
## 自动检测RaycastTarget开关
  通过AssetModificationProcessor类的PrefabStage.prefabSaving。
### 主要脚本：YuToolsUIPrefabHelper.cs
  每次保存预制的时候触发事件，对预制进行遍历自定义要开启RaycastTarget的组件，默认关闭。遍历！还是遍历！
