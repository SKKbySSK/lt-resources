---
marp: true
theme: gaia
class:
  - invert
paginate: true
size: 4:3
---

<!-- _class: lead invert --->

### C#のキモイ高速プログラミング
群馬高専 5年 電子メディア工学科
ぎもちん ([@SKKbySSK_TC](https://twitter.com/SKKbySSK_TC))


---

#### C#には面白い機能がたくさんある
- foreach
- Attribute
- Reflection
- ref struct
- unsafe
- fixed
- GC


---

<!-- _class: lead invert --->
# 高速な配列変換

---

## `unsafe`とは？
- C#でポインタ操作を可能にする
```
int x = 50;
int* ptr = &x; // xのポインタを取り出す
*ptr /= 2; // ポインタの指すデータを2で割る(50 / 2 = 25)

Console.WriteLine(x);
```
Output
```
25
```

---

## 今回やること
byte[]をfloat[]へ変換する

![](conversion.png)

---

## 1. 単純な変換方法
`BitConverter`と`for`を使って変換する

```
const int size = 1024;
byte[] source = new byte[size]; //変換前データ
float[] converted = new float[size / sizeof(float)]; //変換後データ

for (int i = 0; i < converted.Length; i++)
{
    converted[i] = BitConverter.ToSingle(source, i * 4);
}
```

---

## 2. unsafeによる変換方法

```
const int size = 1024;
byte[] source = new byte[size]; //変換前データ
float[] converted = new float[size / sizeof(float)]; //変換後データ

// 配列のアドレスを一時的に固定する
fixed (float* convertedPtr = converted)
fixed (byte* sourcePtr = source)
{
    // byte* -> float*へキャストして4バイトずつ処理する
    float* floatSourcePtr = (float*)sourcePtr;
    for (int i = 0; i < converted.Length; i++)
    {
        convertedPtr[i] = floatSourcePtr[i];
    }
}
```

---

## 結果
- BenchmarkDotNetを使って測定
  - [ArrayConversionTest.cs](Benchmark/ArrayConversionTest.cs)

 方法 |          平均 |
------- |--------------:|
 単純な変換 | 1,517.8060 ns |
 unsafeによる変換 | 1,074.6857 ns |

<!--- _footer: ※ 変換前データを初期化する時間は含んでいません --->

---

## まぁ早くなったけど・・・
- `fixed`に時間がかかる
- 変換後のデータ格納用配列が余計にメモリ喰う

---

<!-- _class: lead invert --->
#### byte配列をfloat配列として使えばええやん

---

<!-- _class: lead invert --->
#### 構造体を使おう！

---

#### 構造体
- C#では構造体のメモリ構造を指定できる
  - Cの`union`みたいなことができる
- `StructLayout`と`FieldOffset`属性を組み合わせる

---

#### 例

```
// メモリ構造を明示することをコンパイラに伝える
[StructLayout(LayoutKind.Explicit)]
struct SampleStruct
{
    [FieldOffset(0)] // 0バイト目にAを配置する
    public int A;

    [FieldOffset(2)] // 2バイト目にBを配置する
    public byte B;
}

SampleStruct data = new SampleStruct();
data.A = 0xDDCCBBAA;
Console.WriteLine(data.B); // 187(0xBB)
```

<!--- _footer: ※エンディアンによっては 204(0xCC) と出る場合もあります --->

---

#### メモリレイアウト
![](sample-struct-layout.png)

---

## 3. Unionもどき

```
[StructLayout(LayoutKind.Explicit)]
struct UnionArray
{
    [FieldOffset(0)]
    public float[] Float;

    [FieldOffset(0)]
    public byte[] Byte;
}

const int size = 1024;
byte[] source = new byte[size]; //変換前データ

UnionArray union = new UnionArray() { Byte = source };
float[] converted = union.Float;
```

---

<!-- _class: lead invert --->
## もう一度計測！！

---

## 結果
- 測定条件は前回と同じ

 方法 |          平均 |
------- |--------------:|
 単純な変換 | 1,517.8060 ns |
 unsafeによる変換 | 1,074.6857 ns |
  Unionもどきによる変換 |     0.6836 ns |

---

## 結果
- 測定条件は前回と同じ

 方法 |          平均 |
------- |--------------:|
 単純な変換 | 1,517.8060 ns |
 unsafeによる変換 | 1,074.6857 ns |
  **Unionもどきによる変換** |     0.6836 ns |

#### 圧倒的パフォーマンス！！

---

## Unionもどきの注意点
- 境界チェックが狂う
  - 配列の長さを記録しているメモリが書き変わらないため
- 型情報も狂う
  - 同様に、型情報のメモリも書き変わらないため

---

## 制約
- Managed型のポインタは取れない
  - GCによりアドレスが実行中に変化するため

  | Managed | Unmanaged |
  | --- | --- |
  | class | int, float等 |
  |       | struct※ |
  |       | enum   |

<!--- _footer: struct※ : managed型を含まない場合のみunmanaged --->

---

