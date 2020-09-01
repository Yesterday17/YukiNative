# YukiNative

## 设计理念

`YUKI` 通过 `node-ffi` 实现了对 `Win32 dll` 的调用。这种调用非常方便，但付出的代价是 YUKI 自身也被局限在了 32 位和 Windows 下。 

诚然，64 位系统可以没有任何障碍地运行 32 位程序，而大部分 `YUKI` 的用户也是 `Windows` 用户。但作为日常 `Windows`、`Linux` 混用，大部分时间都在使用 `Linux` 的用户，这样的设计导致了 `YUKI` 完全没有发挥 `Electron` 跨平台的特点，只起到了替代 `Qt` 的效果。

由此，`YukiNative` 的目标就是将 `YUKI` 中平台相关的部分解耦，把必须由 `Windows` 完成的任务交给 `Windows(Wine)`，而其他部分则交由各个操作系统自行解决。

得益于这种解耦，现在 `YUKI` 可以跟随 `Node` 和 `Electron` 的版本更新了。

## 模式

`YukiNative` 计划有三个模式：`http` 模式、`WebSocket` 模式和 `std` 模式。

### `http` 模式

这种模式是 `http` 与 `WebSocket` 混用的模式。在这种模式下，`YUKI` 通过本地 `http` 请求获取信息，而 `YukiNative` 通过 `WebSocket` 向 `YUKI` 推送信息。

这种模式已经实现，但尚未稳定。

### `WebSocket` 模式

这种模式是纯 `WebSocket` 模式，`YUKI` 仅通过 `WebSocket` 与 `YukiNative` 进行通信。

这种模式尚未实现。

### `std` 模式

这种模式通过 `stdin` 和 `stdout` 进行通信。`YukiNative` 通过 `stdin` 接收请求，并通过 `stdout` 向 `YUKI` 发送信息。

这种模式尚未实现。

## `.NET Framework`？

是的，这个项目使用的是 `.NET Framework` 而不是 `.NET Core`。由于在 `.NET Core` 下无法进行调试，我们只能切换到 `.NET Framework` 下。

不过在代码书写过程中已经充分考虑到了对 `.NET Core` 的兼容性，因此 `.NET Core` 应该可以正常编译。

## `YUKI`

目前 `YUKI` 主仓库并没有对 `YukiNative` 的兼容，并且已暂时停止维护。

实现了 `YukiNative` 兼容的 `YUKI` 可在[该 `fork`](https://github.com/Yesterday17/YUKI-Translator) 中获取。