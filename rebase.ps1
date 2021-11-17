$targets = @(
    'design/geounnyoung'
    'design/seoyoung'
    'design/sunghoon'
    'graphic/haedon'
    'graphic/hyunsung'
    'graphic/jungwan'
    'graphic/junhyo'
    'graphic/kangmin'
    'graphic/kya'
    'graphic/minjae'
    'graphic/minju'
    'graphic/seongjun'
)

foreach ($target in $targets)
{
    Write-Output Target: $target
    git checkout $target
    git rebase dev
    git push
    Write-Output Done.
}