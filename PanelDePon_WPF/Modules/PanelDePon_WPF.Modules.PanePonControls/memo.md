マークダウン記法じゃないけど許して…

パネポン専用コントロールは、共通の項目がいくつかある
・表示倍率
・縦横のセルの個数
・etc...
ので、パネポンコントロール用の共通抽象クラスを作り、共通部分を纏めておこう！

気になるのが、下の方法ができるか

abstract class AbsA{
    // staic !!!!
    protected static int BaseValue = 10;
}

class ClassA : AbsA{
    void ViewBaseValue() => cw(ClassA.BaseValue);
    void ChangeBaseValue() => ClassA.BaseValue = 30;
}
class ClassB : AbsA{
    void ViewBaseValue() => cw(ClassB.BaseValue);
}

var a = new ClassA();
var b = new ClassB();
a.ChangeBaseValue();
a.ViewBaseValue(); // <= たぶん３０
b.ViewBaseValue(); // <= ほんとに３０になりました１！！！！

共通抽象クラスを作るなら、Baseコンストラクタで表示の初期化をしておく
this.Width = ..; this.Height = ..;
でないと表示されない(0)から