éƒ
jC:\Users\yk444\Videos\BTECH\DUMMY\Inkwell\InkWell.Notification\Service\Services\NotificationServiceImpl.cs
	namespace		 	
InkWell		
 
.		 
Notification		 
.		 
Services		 '
.		' (
Services		( 0
{

 
public 

class #
NotificationServiceImpl (
:) * 
INotificationService+ ?
{ 
private #
INotificationRepository '"
notificationRepository( >
;> ?
public #
NotificationServiceImpl &
(& '#
INotificationRepository' >

repository? I
)I J
{ 	"
notificationRepository "
=# $

repository% /
;/ 0
} 	
public 
async 
Task 
< #
NotificationResponseDTO 1
>1 2
Send3 7
(7 8
int8 ;
recipientId< G
,G H
intI L
actorIdM T
,T U
stringV \
type] a
,a b
stringc i
titlej o
,o p
stringq w
messagex 
,	 Ä
int 
	relatedId 
, 
string !
relatedType" -
)- .
{ 	
NotificationModel 
newNotification -
=. /
new0 3
NotificationModel4 E
(E F
)F G
;G H
newNotification 
. 
RecipientId '
=( )
recipientId* 5
;5 6
newNotification 
. 
ActorId #
=$ %
actorId& -
;- .
newNotification 
. 
Type  
=! "
type# '
;' (
newNotification 
. 
Title !
=" #
title$ )
;) *
newNotification 
. 
Message #
=$ %
message& -
;- .
newNotification   
.   
	RelatedId   %
=  & '
	relatedId  ( 1
;  1 2
newNotification!! 
.!! 
RelatedType!! '
=!!( )
relatedType!!* 5
;!!5 6
newNotification"" 
."" 
IsRead"" "
=""# $
false""% *
;""* +
newNotification## 
.## 
	CreatedAt## %
=##& '
DateTime##( 0
.##0 1
UtcNow##1 7
;##7 8
NotificationModel%% 
saved%% #
=%%$ %
await%%& +"
notificationRepository%%, B
.%%B C
Add%%C F
(%%F G
newNotification%%G V
)%%V W
;%%W X
return&& 
MapToDTO&& 
(&& 
saved&& !
)&&! "
;&&" #
}'' 	
public** 
async** 
Task** 
SendBulk** "
(**" #
BroadcastDTO**# /
dto**0 3
)**3 4
{++ 	
if,, 
(,, 
dto,, 
.,, 
RecipientIds,,  
==,,! #
null,,$ (
||,,) +
dto,,, /
.,,/ 0
RecipientIds,,0 <
.,,< =
Count,,= B
==,,C E
$num,,F G
),,G H
{-- 
throw.. 
new.. 
	Exception.. #
(..# $
$str..$ =
)..= >
;..> ?
}// 
foreach11 
(11 
int11 
recipientId11 $
in11% '
dto11( +
.11+ ,
RecipientIds11, 8
)118 9
{22 
NotificationModel33 !
notification33" .
=33/ 0
new331 4
NotificationModel335 F
(33F G
)33G H
;33H I
notification44 
.44 
RecipientId44 (
=44) *
recipientId44+ 6
;446 7
notification66 
.66 
ActorId66 $
=66% &
$num66' (
;66( )
notification77 
.77 
Type77 !
=77" #
$str77$ /
;77/ 0
notification88 
.88 
Title88 "
=88# $
dto88% (
.88( )
Title88) .
;88. /
notification99 
.99 
Message99 $
=99% &
dto99' *
.99* +
Message99+ 2
;992 3
notification:: 
.:: 
	RelatedId:: &
=::' (
$num::) *
;::* +
notification;; 
.;; 
RelatedType;; (
=;;) *
$str;;+ 3
;;;3 4
notification<< 
.<< 
IsRead<< #
=<<$ %
false<<& +
;<<+ ,
notification== 
.== 
	CreatedAt== &
===' (
DateTime==) 1
.==1 2
UtcNow==2 8
;==8 9
await?? "
notificationRepository?? ,
.??, -
Add??- 0
(??0 1
notification??1 =
)??= >
;??> ?
}@@ 
}AA 	
publicDD 
asyncDD 
TaskDD 
<DD 
ListDD 
<DD #
NotificationResponseDTODD 6
>DD6 7
>DD7 8
GetByRecipientDD9 G
(DDG H
intDDH K
recipientIdDDL W
)DDW X
{EE 	
ListFF 
<FF 
NotificationModelFF "
>FF" #
notificationsFF$ 1
=FF2 3
awaitFF4 9"
notificationRepositoryFF: P
.FFP Q
GetByRecipientIdFFQ a
(FFa b
recipientIdFFb m
)FFm n
;FFn o
ListHH 
<HH #
NotificationResponseDTOHH (
>HH( )
resultHH* 0
=HH1 2
newHH3 6
ListHH7 ;
<HH; <#
NotificationResponseDTOHH< S
>HHS T
(HHT U
)HHU V
;HHV W
foreachJJ 
(JJ 
NotificationModelJJ &
notificationJJ' 3
inJJ4 6
notificationsJJ7 D
)JJD E
{KK 
resultLL 
.LL 
AddLL 
(LL 
MapToDTOLL #
(LL# $
notificationLL$ 0
)LL0 1
)LL1 2
;LL2 3
}MM 
returnOO 
resultOO 
;OO 
}PP 	
publicSS 
asyncSS 
TaskSS 
<SS 
ListSS 
<SS #
NotificationResponseDTOSS 6
>SS6 7
>SS7 8
	GetUnreadSS9 B
(SSB C
intSSC F
recipientIdSSG R
)SSR S
{TT 	
ListUU 
<UU 
NotificationModelUU "
>UU" #
notificationsUU$ 1
=UU2 3
awaitUU4 9"
notificationRepositoryUU: P
.VV "
GetUnreadByRecipientIdVV '
(VV' (
recipientIdVV( 3
)VV3 4
;VV4 5
ListXX 
<XX #
NotificationResponseDTOXX (
>XX( )
resultXX* 0
=XX1 2
newXX3 6
ListXX7 ;
<XX; <#
NotificationResponseDTOXX< S
>XXS T
(XXT U
)XXU V
;XXV W
foreachZZ 
(ZZ 
NotificationModelZZ &
notificationZZ' 3
inZZ4 6
notificationsZZ7 D
)ZZD E
{[[ 
result\\ 
.\\ 
Add\\ 
(\\ 
MapToDTO\\ #
(\\# $
notification\\$ 0
)\\0 1
)\\1 2
;\\2 3
}]] 
return__ 
result__ 
;__ 
}`` 	
publiccc 
asynccc 
Taskcc 

MarkAsReadcc $
(cc$ %
intcc% (
notificationIdcc) 7
,cc7 8
intcc9 <
recipientIdcc= H
)ccH I
{dd 	
NotificationModelee 
notificationee *
=ee+ ,
awaitee- 2"
notificationRepositoryee3 I
.eeI J
GetByIdeeJ Q
(eeQ R
notificationIdeeR `
)ee` a
;eea b
ifgg 
(gg 
notificationgg 
==gg 
nullgg  $
)gg$ %
{hh 
throwii 
newii 
	Exceptionii #
(ii# $
$strii$ =
)ii= >
;ii> ?
}jj 
ifmm 
(mm 
notificationmm 
.mm 
RecipientIdmm (
!=mm) +
recipientIdmm, 7
&&mm8 :
notificationmm; G
.mmG H
RecipientIdmmH S
!=mmT V
$nummmW X
)mmX Y
{nn 
throwoo 
newoo 
	Exceptionoo #
(oo# $
$stroo$ W
)ooW X
;ooX Y
}pp 
notificationrr 
.rr 
IsReadrr 
=rr  !
truerr" &
;rr& '
awaitss "
notificationRepositoryss (
.ss( )
Updatess) /
(ss/ 0
notificationss0 <
)ss< =
;ss= >
}tt 	
publicww 
asyncww 
Taskww 
MarkAllReadww %
(ww% &
intww& )
recipientIdww* 5
)ww5 6
{xx 	
Listyy 
<yy 
NotificationModelyy "
>yy" #
unreadyy$ *
=yy+ ,
awaityy- 2"
notificationRepositoryyy3 I
.zz "
GetUnreadByRecipientIdzz '
(zz' (
recipientIdzz( 3
)zz3 4
;zz4 5
foreach|| 
(|| 
NotificationModel|| &
notification||' 3
in||4 6
unread||7 =
)||= >
{}} 
notification~~ 
.~~ 
IsRead~~ #
=~~$ %
true~~& *
;~~* +
await "
notificationRepository ,
., -
Update- 3
(3 4
notification4 @
)@ A
;A B
}
ÄÄ 
}
ÅÅ 	
public
ÑÑ 
async
ÑÑ 
Task
ÑÑ 

DeleteRead
ÑÑ $
(
ÑÑ$ %
int
ÑÑ% (
recipientId
ÑÑ) 4
)
ÑÑ4 5
{
ÖÖ 	
await
áá $
notificationRepository
áá (
.
áá( )*
DeleteByRecipientIdAndIsRead
áá) E
(
ááE F
recipientId
ááF Q
,
ááQ R
true
ááS W
)
ááW X
;
ááX Y
}
àà 	
public
ãã 
async
ãã 
Task
ãã  
DeleteNotification
ãã ,
(
ãã, -
int
ãã- 0
notificationId
ãã1 ?
,
ãã? @
int
ããA D
recipientId
ããE P
)
ããP Q
{
åå 	
NotificationModel
çç 
notification
çç *
=
çç+ ,
await
çç- 2$
notificationRepository
çç3 I
.
ççI J
GetById
ççJ Q
(
ççQ R
notificationId
ççR `
)
çç` a
;
çça b
if
èè 
(
èè 
notification
èè 
==
èè 
null
èè  $
)
èè$ %
{
êê 
throw
ëë 
new
ëë 
	Exception
ëë #
(
ëë# $
$str
ëë$ =
)
ëë= >
;
ëë> ?
}
íí 
if
ïï 
(
ïï 
notification
ïï 
.
ïï 
RecipientId
ïï (
!=
ïï) +
recipientId
ïï, 7
&&
ïï8 :
notification
ïï; G
.
ïïG H
RecipientId
ïïH S
!=
ïïT V
$num
ïïW X
)
ïïX Y
{
ññ 
throw
óó 
new
óó 
	Exception
óó #
(
óó# $
$str
óó$ Q
)
óóQ R
;
óóR S
}
òò 
await
öö $
notificationRepository
öö (
.
öö( )

DeleteById
öö) 3
(
öö3 4
notificationId
öö4 B
)
ööB C
;
ööC D
}
õõ 	
public
ûû 
async
ûû 
Task
ûû 
<
ûû 
int
ûû 
>
ûû 
GetUnreadCount
ûû -
(
ûû- .
int
ûû. 1
recipientId
ûû2 =
)
ûû= >
{
üü 	
int
†† 
count
†† 
=
†† 
await
†† $
notificationRepository
†† 4
.
††4 5&
CountUnreadByRecipientId
††5 M
(
††M N
recipientId
††N Y
)
††Y Z
;
††Z [
return
°° 
count
°° 
;
°° 
}
¢¢ 	
public
•• 
async
•• 
Task
•• 
<
•• 
List
•• 
<
•• %
NotificationResponseDTO
•• 6
>
••6 7
>
••7 8
GetAll
••9 ?
(
••? @
)
••@ A
{
¶¶ 	
List
ßß 
<
ßß 
NotificationModel
ßß "
>
ßß" #
notifications
ßß$ 1
=
ßß2 3
await
ßß4 9$
notificationRepository
ßß: P
.
ßßP Q
GetAll
ßßQ W
(
ßßW X
)
ßßX Y
;
ßßY Z
List
©© 
<
©© %
NotificationResponseDTO
©© (
>
©©( )
result
©©* 0
=
©©1 2
new
©©3 6
List
©©7 ;
<
©©; <%
NotificationResponseDTO
©©< S
>
©©S T
(
©©T U
)
©©U V
;
©©V W
foreach
´´ 
(
´´ 
NotificationModel
´´ &
notification
´´' 3
in
´´4 6
notifications
´´7 D
)
´´D E
{
¨¨ 
result
≠≠ 
.
≠≠ 
Add
≠≠ 
(
≠≠ 
MapToDTO
≠≠ #
(
≠≠# $
notification
≠≠$ 0
)
≠≠0 1
)
≠≠1 2
;
≠≠2 3
}
ÆÆ 
return
∞∞ 
result
∞∞ 
;
∞∞ 
}
±± 	
public
¥¥ 
async
¥¥ 
Task
¥¥  
HandleCommentAdded
¥¥ ,
(
¥¥, -
CommentAddedDTO
¥¥- <
dto
¥¥= @
)
¥¥@ A
{
µµ 	
if
∏∏ 
(
∏∏ 
dto
∏∏ 
.
∏∏ 
NotificationType
∏∏ $
==
∏∏% '
$str
∏∏( 5
)
∏∏5 6
{
ππ 
await
ªª 
Send
ªª 
(
ªª 
recipientId
ºº 
:
ºº  
dto
ºº! $
.
ºº$ %
PostAuthorId
ºº% 1
,
ºº1 2
actorId
ΩΩ 
:
ΩΩ 
dto
ΩΩ  
.
ΩΩ  !
CommentAuthorId
ΩΩ! 0
,
ΩΩ0 1
type
ææ 
:
ææ 
$str
ææ '
,
ææ' (
title
øø 
:
øø 
$"
øø 
{
øø 
dto
øø !
.
øø! "
	ActorName
øø" +
}
øø+ ,
$str
øø, C
"
øøC D
,
øøD E
message
¿¿ 
:
¿¿ 
$"
¿¿ 
{
¿¿  
dto
¿¿  #
.
¿¿# $
	ActorName
¿¿$ -
}
¿¿- .
$str
¿¿. K
"
¿¿K L
,
¿¿L M
	relatedId
¡¡ 
:
¡¡ 
dto
¡¡ "
.
¡¡" #
PostId
¡¡# )
,
¡¡) *
relatedType
¬¬ 
:
¬¬  
$str
¬¬! '
)
√√ 
;
√√ 
}
ƒƒ 
if
»» 
(
»» 
dto
»» 
.
»» 
NotificationType
»» $
==
»»% '
$str
»»( 7
)
»»7 8
{
…… 
await
   
Send
   
(
   
recipientId
ÀÀ 
:
ÀÀ  
dto
ÀÀ! $
.
ÀÀ$ %#
ParentCommentAuthorId
ÀÀ% :
,
ÀÀ: ;
actorId
ÃÃ 
:
ÃÃ 
dto
ÃÃ  
.
ÃÃ  !
CommentAuthorId
ÃÃ! 0
,
ÃÃ0 1
type
ÕÕ 
:
ÕÕ 
$str
ÕÕ )
,
ÕÕ) *
title
ŒŒ 
:
ŒŒ 
$"
ŒŒ 
{
ŒŒ 
dto
ŒŒ !
.
ŒŒ! "
	ActorName
ŒŒ" +
}
ŒŒ+ ,
$str
ŒŒ, D
"
ŒŒD E
,
ŒŒE F
message
œœ 
:
œœ 
$"
œœ 
{
œœ  
dto
œœ  #
.
œœ# $
	ActorName
œœ$ -
}
œœ- .
$str
œœ. G
"
œœG H
,
œœH I
	relatedId
–– 
:
–– 
dto
–– "
.
––" #
	CommentId
––# ,
,
––, -
relatedType
—— 
:
——  
$str
——! *
)
““ 
;
““ 
}
”” 
}
‘‘ 	
public
◊◊ 
async
◊◊ 
Task
◊◊ )
HandlePostLikedPersonalized
◊◊ 5
(
◊◊5 6
int
◊◊6 9
postId
◊◊: @
,
◊◊@ A
int
◊◊B E
postAuthorId
◊◊F R
,
◊◊R S
int
◊◊T W
actorId
◊◊X _
,
◊◊_ `
string
◊◊a g
	actorName
◊◊h q
)
◊◊q r
{
ÿÿ 	
if
⁄⁄ 
(
⁄⁄ 
postAuthorId
⁄⁄ 
==
⁄⁄ 
actorId
⁄⁄  '
)
⁄⁄' (
{
€€ 
return
‹‹ 
;
‹‹ 
}
›› 
await
ﬂﬂ 
Send
ﬂﬂ 
(
ﬂﬂ 
recipientId
‡‡ 
:
‡‡ 
postAuthorId
‡‡ )
,
‡‡) *
actorId
·· 
:
·· 
actorId
··  
,
··  !
type
‚‚ 
:
‚‚ 
$str
‚‚ 
,
‚‚ 
title
„„ 
:
„„ 
$"
„„ 
{
„„ 
	actorName
„„ #
}
„„# $
$str
„„$ 4
"
„„4 5
,
„„5 6
message
‰‰ 
:
‰‰ 
$"
‰‰ 
{
‰‰ 
	actorName
‰‰ %
}
‰‰% &
$str
‰‰& 7
"
‰‰7 8
,
‰‰8 9
	relatedId
ÂÂ 
:
ÂÂ 
postId
ÂÂ !
,
ÂÂ! "
relatedType
ÊÊ 
:
ÊÊ 
$str
ÊÊ #
)
ÁÁ 
;
ÁÁ 
}
ËË 	
public
ÎÎ 
async
ÎÎ 
Task
ÎÎ !
HandlePostPublished
ÎÎ -
(
ÎÎ- .
int
ÎÎ. 1
postId
ÎÎ2 8
,
ÎÎ8 9
string
ÎÎ: @
title
ÎÎA F
,
ÎÎF G
int
ÎÎH K
authorId
ÎÎL T
)
ÎÎT U
{
ÏÏ 	
await
ÓÓ 
Send
ÓÓ 
(
ÓÓ 
recipientId
ÔÔ 
:
ÔÔ 
$num
ÔÔ 
,
ÔÔ 
actorId
 
:
 
authorId
 !
,
! "
type
ÒÒ 
:
ÒÒ 
$str
ÒÒ  
,
ÒÒ  !
title
ÚÚ 
:
ÚÚ 
$str
ÚÚ ,
,
ÚÚ, -
message
ÛÛ 
:
ÛÛ 
$"
ÛÛ 
$str
ÛÛ 5
{
ÛÛ5 6
title
ÛÛ6 ;
}
ÛÛ; <
"
ÛÛ< =
,
ÛÛ= >
	relatedId
ÙÙ 
:
ÙÙ 
postId
ÙÙ !
,
ÙÙ! "
relatedType
ıı 
:
ıı 
$str
ıı #
)
ˆˆ 
;
ˆˆ 
}
˜˜ 	
public
˙˙ 
async
˙˙ 
Task
˙˙ 
HandleMention
˙˙ '
(
˙˙' (
int
˙˙( +
mentionedUserId
˙˙, ;
,
˙˙; <
int
˙˙= @
actorId
˙˙A H
,
˙˙H I
int
˚˚ 
	commentId
˚˚ 
,
˚˚ 
int
˚˚ 
postId
˚˚ %
)
˚˚% &
{
¸¸ 	
if
˛˛ 
(
˛˛ 
mentionedUserId
˛˛ 
==
˛˛  "
actorId
˛˛# *
)
˛˛* +
{
ˇˇ 
return
ÄÄ 
;
ÄÄ 
}
ÅÅ 
await
ÉÉ 
Send
ÉÉ 
(
ÉÉ 
recipientId
ÑÑ 
:
ÑÑ 
mentionedUserId
ÑÑ ,
,
ÑÑ, -
actorId
ÖÖ 
:
ÖÖ 
actorId
ÖÖ  
,
ÖÖ  !
type
ÜÜ 
:
ÜÜ 
$str
ÜÜ 
,
ÜÜ  
title
áá 
:
áá 
$str
áá 8
,
áá8 9
message
àà 
:
àà 
$str
àà >
,
àà> ?
	relatedId
ââ 
:
ââ 
	commentId
ââ $
,
ââ$ %
relatedType
ää 
:
ää 
$str
ää &
)
ãã 
;
ãã 
}
åå 	
public
èè 
async
èè 
Task
èè 
<
èè 
List
èè 
<
èè %
NotificationResponseDTO
èè 6
>
èè6 7
>
èè7 8
	GetByType
èè9 B
(
èèB C
string
èèC I
type
èèJ N
)
èèN O
{
êê 	
List
ëë 
<
ëë 
NotificationModel
ëë "
>
ëë" #
notifications
ëë$ 1
=
ëë2 3
await
ëë4 9$
notificationRepository
ëë: P
.
ëëP Q
	GetByType
ëëQ Z
(
ëëZ [
type
ëë[ _
)
ëë_ `
;
ëë` a
List
íí 
<
íí %
NotificationResponseDTO
íí (
>
íí( )
result
íí* 0
=
íí1 2
new
íí3 6
List
íí7 ;
<
íí; <%
NotificationResponseDTO
íí< S
>
ííS T
(
ííT U
)
ííU V
;
ííV W
foreach
îî 
(
îî 
NotificationModel
îî &
notification
îî' 3
in
îî4 6
notifications
îî7 D
)
îîD E
{
ïï 
result
ññ 
.
ññ 
Add
ññ 
(
ññ 
MapToDTO
ññ #
(
ññ# $
notification
ññ$ 0
)
ññ0 1
)
ññ1 2
;
ññ2 3
}
óó 
return
òò 
result
òò 
;
òò 
}
ôô 	
public
úú 
async
úú 
Task
úú 
<
úú 
List
úú 
<
úú %
NotificationResponseDTO
úú 6
>
úú6 7
>
úú7 8
GetByRelatedId
úú9 G
(
úúG H
int
úúH K
	relatedId
úúL U
)
úúU V
{
ùù 	
List
ûû 
<
ûû 
NotificationModel
ûû "
>
ûû" #
notifications
ûû$ 1
=
ûû2 3
await
ûû4 9$
notificationRepository
ûû: P
.
ûûP Q
GetByRelatedId
ûûQ _
(
ûû_ `
	relatedId
ûû` i
)
ûûi j
;
ûûj k
List
üü 
<
üü %
NotificationResponseDTO
üü (
>
üü( )
result
üü* 0
=
üü1 2
new
üü3 6
List
üü7 ;
<
üü; <%
NotificationResponseDTO
üü< S
>
üüS T
(
üüT U
)
üüU V
;
üüV W
foreach
°° 
(
°° 
NotificationModel
°° &
notification
°°' 3
in
°°4 6
notifications
°°7 D
)
°°D E
{
¢¢ 
result
££ 
.
££ 
Add
££ 
(
££ 
MapToDTO
££ #
(
££# $
notification
££$ 0
)
££0 1
)
££1 2
;
££2 3
}
§§ 
return
•• 
result
•• 
;
•• 
}
¶¶ 	
private
©© %
NotificationResponseDTO
©© '
MapToDTO
©©( 0
(
©©0 1
NotificationModel
©©1 B
notification
©©C O
)
©©O P
{
™™ 	%
NotificationResponseDTO
´´ #
dto
´´$ '
=
´´( )
new
´´* -%
NotificationResponseDTO
´´. E
(
´´E F
)
´´F G
;
´´G H
dto
¨¨ 
.
¨¨ 
NotificationId
¨¨ 
=
¨¨  
notification
¨¨! -
.
¨¨- .
NotificationId
¨¨. <
;
¨¨< =
dto
≠≠ 
.
≠≠ 
RecipientId
≠≠ 
=
≠≠ 
notification
≠≠ *
.
≠≠* +
RecipientId
≠≠+ 6
;
≠≠6 7
dto
ÆÆ 
.
ÆÆ 
ActorId
ÆÆ 
=
ÆÆ 
notification
ÆÆ &
.
ÆÆ& '
ActorId
ÆÆ' .
;
ÆÆ. /
dto
ØØ 
.
ØØ 
Type
ØØ 
=
ØØ 
notification
ØØ #
.
ØØ# $
Type
ØØ$ (
;
ØØ( )
dto
∞∞ 
.
∞∞ 
Title
∞∞ 
=
∞∞ 
notification
∞∞ $
.
∞∞$ %
Title
∞∞% *
;
∞∞* +
dto
±± 
.
±± 
Message
±± 
=
±± 
notification
±± &
.
±±& '
Message
±±' .
;
±±. /
dto
≤≤ 
.
≤≤ 
	RelatedId
≤≤ 
=
≤≤ 
notification
≤≤ (
.
≤≤( )
	RelatedId
≤≤) 2
;
≤≤2 3
dto
≥≥ 
.
≥≥ 
RelatedType
≥≥ 
=
≥≥ 
notification
≥≥ *
.
≥≥* +
RelatedType
≥≥+ 6
;
≥≥6 7
dto
¥¥ 
.
¥¥ 
IsRead
¥¥ 
=
¥¥ 
notification
¥¥ %
.
¥¥% &
IsRead
¥¥& ,
;
¥¥, -
dto
µµ 
.
µµ 
	CreatedAt
µµ 
=
µµ 
notification
µµ (
.
µµ( )
	CreatedAt
µµ) 2
;
µµ2 3
return
∂∂ 
dto
∂∂ 
;
∂∂ 
}
∑∑ 	
}
∏∏ 
}ππ √
iC:\Users\yk444\Videos\BTECH\DUMMY\Inkwell\InkWell.Notification\Service\Interfaces\INotificationService.cs
	namespace 	
InkWell
 
. 
Notification 
. 
Services '
.' (

Interfaces( 2
{ 
public 

	interface  
INotificationService )
{ 
Task

 
<

 #
NotificationResponseDTO

 $
>

$ %
Send

& *
(

* +
int

+ .
recipientId

/ :
,

: ;
int

< ?
actorId

@ G
,

G H
string 
type 
, 
string 
title  %
,% &
string' -
message. 5
,5 6
int 
	relatedId 
, 
string !
relatedType" -
)- .
;. /
Task 
SendBulk 
( 
BroadcastDTO "
dto# &
)& '
;' (
Task 
< 
List 
< #
NotificationResponseDTO )
>) *
>* +
GetByRecipient, :
(: ;
int; >
recipientId? J
)J K
;K L
Task 
< 
List 
< #
NotificationResponseDTO )
>) *
>* +
	GetUnread, 5
(5 6
int6 9
recipientId: E
)E F
;F G
Task 

MarkAsRead 
( 
int 
notificationId *
,* +
int, /
recipientId0 ;
); <
;< =
Task 
MarkAllRead 
( 
int 
recipientId (
)( )
;) *
Task 

DeleteRead 
( 
int 
recipientId '
)' (
;( )
Task!! 
DeleteNotification!! 
(!!  
int!!  #
notificationId!!$ 2
,!!2 3
int!!4 7
recipientId!!8 C
)!!C D
;!!D E
Task$$ 
<$$ 
int$$ 
>$$ 
GetUnreadCount$$  
($$  !
int$$! $
recipientId$$% 0
)$$0 1
;$$1 2
Task'' 
<'' 
List'' 
<'' #
NotificationResponseDTO'' )
>'') *
>''* +
GetAll'', 2
(''2 3
)''3 4
;''4 5
Task** 
HandleCommentAdded** 
(**  
CommentAddedDTO**  /
dto**0 3
)**3 4
;**4 5
Task-- '
HandlePostLikedPersonalized-- (
(--( )
int--) ,
postId--- 3
,--3 4
int--5 8
postAuthorId--9 E
,--E F
int--G J
actorId--K R
,--R S
string--T Z
	actorName--[ d
)--d e
;--e f
Task00 
HandleMention00 
(00 
int00 
mentionedUserId00 .
,00. /
int000 3
actorId004 ;
,00; <
int00= @
	commentId00A J
,00J K
int00L O
postId00P V
)00V W
;00W X
Task33 
HandlePostPublished33  
(33  !
int33! $
postId33% +
,33+ ,
string33- 3
title334 9
,339 :
int33; >
authorId33? G
)33G H
;33H I
Task66 
<66 
List66 
<66 #
NotificationResponseDTO66 )
>66) *
>66* +
	GetByType66, 5
(665 6
string666 <
type66= A
)66A B
;66B C
Task99 
<99 
List99 
<99 #
NotificationResponseDTO99 )
>99) *
>99* +
GetByRelatedId99, :
(99: ;
int99; >
	relatedId99? H
)99H I
;99I J
}:: 
};; Ó[
tC:\Users\yk444\Videos\BTECH\DUMMY\Inkwell\InkWell.Notification\Repository\Repositories\NotificationRepositoryImpl.cs
	namespace		 	
InkWell		
 
.		 
Notification		 
.		 

Repository		 )
.		) *
Repositories		* 6
{

 
public 

class &
NotificationRepositoryImpl +
:, -#
INotificationRepository. E
{ 
private !
NotificationDbContext %
	dbContext& /
;/ 0
public &
NotificationRepositoryImpl )
() *!
NotificationDbContext* ?
context@ G
)G H
{ 	
	dbContext 
= 
context 
;  
} 	
public 
async 
Task 
< 
NotificationModel +
>+ ,
GetById- 4
(4 5
int5 8
id9 ;
); <
{ 	
NotificationModel 
notification *
=+ ,
await- 2
	dbContext3 <
.< =
Notifications= J
.J K
	FindAsyncK T
(T U
idU W
)W X
;X Y
return 
notification 
;  
} 	
public 
async 
Task 
< 
List 
< 
NotificationModel 0
>0 1
>1 2
GetByRecipientId3 C
(C D
intD G
recipientIdH S
)S T
{ 	
List   
<   
NotificationModel   "
>  " #
notifications  $ 1
=  2 3
await  4 9
	dbContext  : C
.  C D
Notifications  D Q
.!! 
Where!! 
(!! 
n!! 
=>!! 
n!! 
.!! 
RecipientId!! )
==!!* ,
recipientId!!- 8
||!!9 ;
n!!< =
.!!= >
RecipientId!!> I
==!!J L
$num!!M N
)!!N O
."" 
OrderByDescending"" "
(""" #
n""# $
=>""% '
n""( )
."") *
	CreatedAt""* 3
)""3 4
.## 
ToListAsync## 
(## 
)## 
;## 
return$$ 
notifications$$  
;$$  !
}%% 	
public(( 
async(( 
Task(( 
<(( 
List(( 
<(( 
NotificationModel(( 0
>((0 1
>((1 2"
GetUnreadByRecipientId((3 I
(((I J
int((J M
recipientId((N Y
)((Y Z
{)) 	
List** 
<** 
NotificationModel** "
>**" #
notifications**$ 1
=**2 3
await**4 9
	dbContext**: C
.**C D
Notifications**D Q
.++ 
Where++ 
(++ 
n++ 
=>++ 
(++ 
n++ 
.++ 
RecipientId++ *
==+++ -
recipientId++. 9
||++: <
n++= >
.++> ?
RecipientId++? J
==++K M
$num++N O
)++O P
&&++Q S
n++T U
.++U V
IsRead++V \
==++] _
false++` e
)++e f
.,, 
OrderByDescending,, "
(,," #
n,,# $
=>,,% '
n,,( )
.,,) *
	CreatedAt,,* 3
),,3 4
.-- 
ToListAsync-- 
(-- 
)-- 
;-- 
return.. 
notifications..  
;..  !
}// 	
public22 
async22 
Task22 
<22 
List22 
<22 
NotificationModel22 0
>220 1
>221 2
	GetByType223 <
(22< =
string22= C
type22D H
)22H I
{33 	
List44 
<44 
NotificationModel44 "
>44" #
notifications44$ 1
=442 3
await444 9
	dbContext44: C
.44C D
Notifications44D Q
.55 
Where55 
(55 
n55 
=>55 
n55 
.55 
Type55 "
==55# %
type55& *
)55* +
.66 
OrderByDescending66 "
(66" #
n66# $
=>66% '
n66( )
.66) *
	CreatedAt66* 3
)663 4
.77 
ToListAsync77 
(77 
)77 
;77 
return88 
notifications88  
;88  !
}99 	
public<< 
async<< 
Task<< 
<<< 
List<< 
<<< 
NotificationModel<< 0
><<0 1
><<1 2
GetByRelatedId<<3 A
(<<A B
int<<B E
	relatedId<<F O
)<<O P
{== 	
List>> 
<>> 
NotificationModel>> "
>>>" #
notifications>>$ 1
=>>2 3
await>>4 9
	dbContext>>: C
.>>C D
Notifications>>D Q
.?? 
Where?? 
(?? 
n?? 
=>?? 
n?? 
.?? 
	RelatedId?? '
==??( *
	relatedId??+ 4
)??4 5
.@@ 
OrderByDescending@@ "
(@@" #
n@@# $
=>@@% '
n@@( )
.@@) *
	CreatedAt@@* 3
)@@3 4
.AA 
ToListAsyncAA 
(AA 
)AA 
;AA 
returnBB 
notificationsBB  
;BB  !
}CC 	
publicFF 
asyncFF 
TaskFF 
<FF 
intFF 
>FF $
CountUnreadByRecipientIdFF 7
(FF7 8
intFF8 ;
recipientIdFF< G
)FFG H
{GG 	
intHH 
countHH 
=HH 
awaitHH 
	dbContextHH '
.HH' (
NotificationsHH( 5
.II 

CountAsyncII 
(II 
nII 
=>II  
(II! "
nII" #
.II# $
RecipientIdII$ /
==II0 2
recipientIdII3 >
||II? A
nIIB C
.IIC D
RecipientIdIID O
==IIP R
$numIIS T
)IIT U
&&IIV X
nIIY Z
.IIZ [
IsReadII[ a
==IIb d
falseIIe j
)IIj k
;IIk l
returnJJ 
countJJ 
;JJ 
}KK 	
publicNN 
asyncNN 
TaskNN 
<NN 
ListNN 
<NN 
NotificationModelNN 0
>NN0 1
>NN1 2
GetAllNN3 9
(NN9 :
)NN: ;
{OO 	
ListPP 
<PP 
NotificationModelPP "
>PP" #
notificationsPP$ 1
=PP2 3
awaitPP4 9
	dbContextPP: C
.PPC D
NotificationsPPD Q
.QQ 
OrderByDescendingQQ "
(QQ" #
nQQ# $
=>QQ% '
nQQ( )
.QQ) *
	CreatedAtQQ* 3
)QQ3 4
.RR 
ToListAsyncRR 
(RR 
)RR 
;RR 
returnSS 
notificationsSS  
;SS  !
}TT 	
publicWW 
asyncWW 
TaskWW 
<WW 
NotificationModelWW +
>WW+ ,
AddWW- 0
(WW0 1
NotificationModelWW1 B
notificationWWC O
)WWO P
{XX 	
	dbContextYY 
.YY 
NotificationsYY #
.YY# $
AddYY$ '
(YY' (
notificationYY( 4
)YY4 5
;YY5 6
awaitZZ 
	dbContextZZ 
.ZZ 
SaveChangesAsyncZZ ,
(ZZ, -
)ZZ- .
;ZZ. /
return[[ 
notification[[ 
;[[  
}\\ 	
public__ 
async__ 
Task__ 
<__ 
NotificationModel__ +
>__+ ,
Update__- 3
(__3 4
NotificationModel__4 E
notification__F R
)__R S
{`` 	
	dbContextaa 
.aa 
Notificationsaa #
.aa# $
Updateaa$ *
(aa* +
notificationaa+ 7
)aa7 8
;aa8 9
awaitbb 
	dbContextbb 
.bb 
SaveChangesAsyncbb ,
(bb, -
)bb- .
;bb. /
returncc 
notificationcc 
;cc  
}dd 	
publicgg 
asyncgg 
Taskgg 

DeleteByIdgg $
(gg$ %
intgg% (
idgg) +
)gg+ ,
{hh 	
NotificationModelii 
notificationii *
=ii+ ,
awaitii- 2
	dbContextii3 <
.ii< =
Notificationsii= J
.iiJ K
	FindAsynciiK T
(iiT U
idiiU W
)iiW X
;iiX Y
ifjj 
(jj 
notificationjj 
!=jj 
nulljj  $
)jj$ %
{kk 
	dbContextll 
.ll 
Notificationsll '
.ll' (
Removell( .
(ll. /
notificationll/ ;
)ll; <
;ll< =
awaitmm 
	dbContextmm 
.mm  
SaveChangesAsyncmm  0
(mm0 1
)mm1 2
;mm2 3
}nn 
}oo 	
publicss 
asyncss 
Taskss #
DeleteReadByRecipientIdss 1
(ss1 2
intss2 5
recipientIdss6 A
)ssA B
{tt 	
Listuu 
<uu 
NotificationModeluu "
>uu" #
readNotificationsuu$ 5
=uu6 7
awaituu8 =
	dbContextuu> G
.uuG H
NotificationsuuH U
.vv 
Wherevv 
(vv 
nvv 
=>vv 
nvv 
.vv 
RecipientIdvv )
==vv* ,
recipientIdvv- 8
&&vv9 ;
nvv< =
.vv= >
IsReadvv> D
==vvE G
truevvH L
)vvL M
.ww 
ToListAsyncww 
(ww 
)ww 
;ww 
	dbContextyy 
.yy 
Notificationsyy #
.yy# $
RemoveRangeyy$ /
(yy/ 0
readNotificationsyy0 A
)yyA B
;yyB C
awaitzz 
	dbContextzz 
.zz 
SaveChangesAsynczz ,
(zz, -
)zz- .
;zz. /
}{{ 	
public~~ 
async~~ 
Task~~ (
DeleteByRecipientIdAndIsRead~~ 6
(~~6 7
int~~7 :
recipientId~~; F
,~~F G
bool~~H L
isRead~~M S
)~~S T
{ 	
List
ÄÄ 
<
ÄÄ 
NotificationModel
ÄÄ "
>
ÄÄ" #
notifications
ÄÄ$ 1
=
ÄÄ2 3
await
ÄÄ4 9
	dbContext
ÄÄ: C
.
ÄÄC D
Notifications
ÄÄD Q
.
ÅÅ 
Where
ÅÅ 
(
ÅÅ 
n
ÅÅ 
=>
ÅÅ 
n
ÅÅ 
.
ÅÅ 
RecipientId
ÅÅ )
==
ÅÅ* ,
recipientId
ÅÅ- 8
&&
ÅÅ9 ;
n
ÅÅ< =
.
ÅÅ= >
IsRead
ÅÅ> D
==
ÅÅE G
isRead
ÅÅH N
)
ÅÅN O
.
ÇÇ 
ToListAsync
ÇÇ 
(
ÇÇ 
)
ÇÇ 
;
ÇÇ 
	dbContext
ÑÑ 
.
ÑÑ 
Notifications
ÑÑ #
.
ÑÑ# $
RemoveRange
ÑÑ$ /
(
ÑÑ/ 0
notifications
ÑÑ0 =
)
ÑÑ= >
;
ÑÑ> ?
await
ÖÖ 
	dbContext
ÖÖ 
.
ÖÖ 
SaveChangesAsync
ÖÖ ,
(
ÖÖ, -
)
ÖÖ- .
;
ÖÖ. /
}
ÜÜ 	
}
áá 
}àà Ó
oC:\Users\yk444\Videos\BTECH\DUMMY\Inkwell\InkWell.Notification\Repository\Interfaces\INotificationRepository.cs
	namespace 	
InkWell
 
. 
Notification 
. 

Repository )
.) *

Interfaces* 4
{ 
public 

	interface #
INotificationRepository ,
{ 
Task 
< 
NotificationModel 
> 
GetById  '
(' (
int( +
id, .
). /
;/ 0
Task 
< 
List 
< 
NotificationModel #
># $
>$ %
GetByRecipientId& 6
(6 7
int7 :
recipientId; F
)F G
;G H
Task 
< 
List 
< 
NotificationModel #
># $
>$ %"
GetUnreadByRecipientId& <
(< =
int= @
recipientIdA L
)L M
;M N
Task 
< 
List 
< 
NotificationModel #
># $
>$ %
	GetByType& /
(/ 0
string0 6
type7 ;
); <
;< =
Task 
< 
List 
< 
NotificationModel #
># $
>$ %
GetByRelatedId& 4
(4 5
int5 8
	relatedId9 B
)B C
;C D
Task 
< 
int 
> $
CountUnreadByRecipientId *
(* +
int+ .
recipientId/ :
): ;
;; <
Task 
< 
List 
< 
NotificationModel #
># $
>$ %
GetAll& ,
(, -
)- .
;. /
Task 
< 
NotificationModel 
> 
Add  #
(# $
NotificationModel$ 5
notification6 B
)B C
;C D
Task   
<   
NotificationModel   
>   
Update    &
(  & '
NotificationModel  ' 8
notification  9 E
)  E F
;  F G
Task## 

DeleteById## 
(## 
int## 
id## 
)## 
;##  
Task&& #
DeleteReadByRecipientId&& $
(&&$ %
int&&% (
recipientId&&) 4
)&&4 5
;&&5 6
Task** (
DeleteByRecipientIdAndIsRead** )
(**) *
int*** -
recipientId**. 9
,**9 :
bool**; ?
isRead**@ F
)**F G
;**G H
}++ 
},, ÛO
IC:\Users\yk444\Videos\BTECH\DUMMY\Inkwell\InkWell.Notification\Program.cs
var 
builder 
= 
WebApplication 
. 
CreateBuilder *
(* +
args+ /
)/ 0
;0 1
var 
configuration 
= 
builder 
. 
Configuration )
;) *
builder 
. 
Services 
. 
AddDbContext 
< !
NotificationDbContext 3
>3 4
(4 5
options5 <
=>= ?
{ 
options 
. 
	UseNpgsql 
( 
configuration #
.# $
GetConnectionString$ 7
(7 8
$str8 H
)H I
)I J
;J K
} 
) 
; 
builder 
. 
Services 
. 
AddMassTransit 
(  
x  !
=>" $
{ 
x 
. 
AddConsumer 
< 
InkWell 
. 
Notification &
.& '
	Messaging' 0
.0 1
	Consumers1 :
.: ;
PostLikedConsumer; L
>L M
(M N
)N O
;O P
x 
. 
AddConsumer 
< 
InkWell 
. 
Notification &
.& '
	Messaging' 0
.0 1
	Consumers1 :
.: ; 
CommentAddedConsumer; O
>O P
(P Q
)Q R
;R S
x 
. 
AddConsumer 
< 
InkWell 
. 
Notification &
.& '
	Messaging' 0
.0 1
	Consumers1 :
.: ;!
PostPublishedConsumer; P
>P Q
(Q R
)R S
;S T
x   
.   
AddConsumer   
<   
InkWell   
.   
Notification   &
.  & '
	Messaging  ' 0
.  0 1
	Consumers  1 :
.  : ;'
NewsletterPublishedConsumer  ; V
>  V W
(  W X
)  X Y
;  Y Z
x"" 
."" 
UsingRabbitMq"" 
("" 
("" 
context"" 
,"" 
cfg"" !
)""! "
=>""# %
{## 
cfg$$ 
.$$ 
Host$$ 
($$ 
configuration$$ 
[$$ 
$str$$ .
]$$. /
,$$/ 0
$str$$1 4
,$$4 5
h$$6 7
=>$$8 :
{$$; <
}$$= >
)$$> ?
;$$? @
cfg%% 
.%% 
ConfigureEndpoints%% 
(%% 
context%% &
)%%& '
;%%' (
}&& 
)&& 
;&& 
}'' 
)'' 
;'' 
builder++ 
.++ 
Services++ 
.++ 
	AddScoped++ 
<++ #
INotificationRepository++ 2
,++2 3&
NotificationRepositoryImpl++4 N
>++N O
(++O P
)++P Q
;++Q R
builder,, 
.,, 
Services,, 
.,, 
	AddScoped,, 
<,,  
INotificationService,, /
,,,/ 0#
NotificationServiceImpl,,1 H
>,,H I
(,,I J
),,J K
;,,K L
byte// 
[// 
]// 
keyBytes// 
=// 
Encoding// 
.// 
UTF8// 
.//  
GetBytes//  (
(//( )
configuration//) 6
[//6 7
$str//7 C
]//C D
)//D E
;//E F
var00 
securityKey00 
=00 
new00  
SymmetricSecurityKey00 *
(00* +
keyBytes00+ 3
)003 4
;004 5
builder22 
.22 
Services22 
.22 
AddAuthentication22 "
(22" #
options22# *
=>22+ -
{33 
options44 
.44 %
DefaultAuthenticateScheme44 %
=44& '
JwtBearerDefaults44( 9
.449 : 
AuthenticationScheme44: N
;44N O
options55 
.55 "
DefaultChallengeScheme55 "
=55# $
JwtBearerDefaults55% 6
.556 7 
AuthenticationScheme557 K
;55K L
}66 
)66 
.77 
AddJwtBearer77 
(77 
options77 
=>77 
{88 
options99 
.99 %
TokenValidationParameters99 %
=99& '
new99( +%
TokenValidationParameters99, E
{:: $
ValidateIssuerSigningKey;;  
=;;! "
true;;# '
,;;' (
IssuerSigningKey<< 
=<< 
securityKey<< &
,<<& '
ValidateIssuer== 
=== 
true== 
,== 
ValidIssuer>> 
=>> 
configuration>> #
[>># $
$str>>$ 0
]>>0 1
,>>1 2
ValidateAudience?? 
=?? 
true?? 
,??  
ValidAudience@@ 
=@@ 
configuration@@ %
[@@% &
$str@@& 4
]@@4 5
,@@5 6
ValidateLifetimeAA 
=AA 
trueAA 
}BB 
;BB 
}CC 
)CC 
;CC 
builderFF 
.FF 
ServicesFF 
.FF 
AddAuthorizationFF !
(FF! "
)FF" #
;FF# $
builderII 
.II 
ServicesII 
.II 
AddCorsII 
(II 
optionsII  
=>II! #
{JJ 
optionsKK 
.KK 
	AddPolicyKK 
(KK 
$strKK $
,KK$ %
policyKK& ,
=>KK- /
{LL 
policyMM 
.MM 
WithOriginsMM 
(MM 
$strMM 2
)MM2 3
.NN 
AllowAnyHeaderNN 
(NN 
)NN 
.OO 
AllowAnyMethodOO 
(OO 
)OO 
;OO  
}PP 
)PP 
;PP 
}QQ 
)QQ 
;QQ 
builderTT 
.TT 
ServicesTT 
.TT 
AddControllersTT 
(TT  
)TT  !
;TT! "
builderUU 
.UU 
ServicesUU 
.UU #
AddEndpointsApiExplorerUU (
(UU( )
)UU) *
;UU* +
builderXX 
.XX 
ServicesXX 
.XX 
AddSwaggerGenXX 
(XX 
optionsXX &
=>XX' )
{YY 
optionsZZ 
.ZZ !
AddSecurityDefinitionZZ !
(ZZ! "
$strZZ" *
,ZZ* +
newZZ, /!
OpenApiSecuritySchemeZZ0 E
{[[ 
Name\\ 
=\\ 
$str\\ 
,\\ 
Type]] 
=]] 
SecuritySchemeType]] !
.]]! "
Http]]" &
,]]& '
Scheme^^ 
=^^ 
$str^^ 
,^^ 
BearerFormat__ 
=__ 
$str__ 
,__ 
In`` 

=`` 
ParameterLocation`` 
.`` 
Header`` %
,``% &
Descriptionaa 
=aa 
$straa '
}bb 
)bb 
;bb 
optionsdd 
.dd "
AddSecurityRequirementdd "
(dd" #
newdd# &&
OpenApiSecurityRequirementdd' A
{ee 
{ff 	
newgg !
OpenApiSecuritySchemegg %
{hh 
	Referenceii 
=ii 
newii 
OpenApiReferenceii  0
{jj 
Typekk 
=kk 
ReferenceTypekk (
.kk( )
SecuritySchemekk) 7
,kk7 8
Idll 
=ll 
$strll !
}mm 
}nn 
,nn 
newoo 
stringoo 
[oo 
]oo 
{oo 
}oo 
}pp 	
}qq 
)qq 
;qq 
}rr 
)rr 
;rr 
vartt 
apptt 
=tt 	
buildertt
 
.tt 
Buildtt 
(tt 
)tt 
;tt 
ifww 
(ww 
appww 
.ww 
Environmentww 
.ww 
IsDevelopmentww !
(ww! "
)ww" #
)ww# $
{xx 
appyy 
.yy 

UseSwaggeryy 
(yy 
)yy 
;yy 
appzz 
.zz 
UseSwaggerUIzz 
(zz 
)zz 
;zz 
}{{ 
app}} 
.}} 
UseCors}} 
(}} 
$str}} 
)}} 
;}} 
app~~ 
.~~ 

UseRouting~~ 
(~~ 
)~~ 
;~~ 
app 
. 
UseAuthentication 
( 
) 
; 
appÄÄ 
.
ÄÄ 
UseAuthorization
ÄÄ 
(
ÄÄ 
)
ÄÄ 
;
ÄÄ 
appÅÅ 
.
ÅÅ 
MapControllers
ÅÅ 
(
ÅÅ 
)
ÅÅ 
;
ÅÅ 
usingÑÑ 
(
ÑÑ 
var
ÑÑ 

scope
ÑÑ 
=
ÑÑ 
app
ÑÑ 
.
ÑÑ 
Services
ÑÑ 
.
ÑÑ  
CreateScope
ÑÑ  +
(
ÑÑ+ ,
)
ÑÑ, -
)
ÑÑ- .
{ÖÖ 
var
ÜÜ 
db
ÜÜ 

=
ÜÜ 
scope
ÜÜ 
.
ÜÜ 
ServiceProvider
ÜÜ "
.
ÜÜ" # 
GetRequiredService
ÜÜ# 5
<
ÜÜ5 6#
NotificationDbContext
ÜÜ6 K
>
ÜÜK L
(
ÜÜL M
)
ÜÜM N
;
ÜÜN O
db
áá 
.
áá 
Database
áá 
.
áá 
EnsureCreated
áá 
(
áá 
)
áá 
;
áá  
}àà 
appää 
.
ää 
Run
ää 
(
ää 
)
ää 	
;
ää	 
√
ZC:\Users\yk444\Videos\BTECH\DUMMY\Inkwell\InkWell.Notification\Models\NotificationModel.cs
	namespace 	
InkWell
 
. 
Notification 
. 
Models %
{ 
public 

class 
NotificationModel "
{ 
[ 	
Key	 
] 
public		 
int		 
NotificationId		 !
{		" #
get		$ '
;		' (
set		) ,
;		, -
}		. /
public 
int 
RecipientId 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 
int 
ActorId 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Type 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Title 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
Message 
{ 
get  #
;# $
set% (
;( )
}* +
public 
int 
	RelatedId 
{ 
get "
;" #
set$ '
;' (
}) *
public 
string 
RelatedType !
{" #
get$ '
;' (
set) ,
;, -
}. /
public"" 
bool"" 
IsRead"" 
{"" 
get""  
;""  !
set""" %
;""% &
}""' (
public%% 
DateTime%% 
	CreatedAt%% !
{%%" #
get%%$ '
;%%' (
set%%) ,
;%%, -
}%%. /
public(( 
NotificationModel((  
(((  !
)((! "
{)) 	
Type** 
=** 
$str** 
;** 
Title++ 
=++ 
$str++ 
;++ 
Message,, 
=,, 
$str,, 
;,, 
RelatedType-- 
=-- 
$str-- 
;-- 
IsRead.. 
=.. 
false.. 
;.. 
	CreatedAt// 
=// 
DateTime//  
.//  !
UtcNow//! '
;//' (
}00 	
}11 
}22 ∑
eC:\Users\yk444\Videos\BTECH\DUMMY\Inkwell\InkWell.Notification\Messaging\Events\PostPublishedEvent.cs
	namespace 	
InkWell
 
. 
Shared 
. 
Events 
{ 
public 

class 
PostPublishedEvent #
{ 
public 
int 
PostId 
{ 
get 
;  
set! $
;$ %
}& '
public 
string 
Title 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
Slug 
{ 
get  
;  !
set" %
;% &
}' (
public 
int 
AuthorId 
{ 
get !
;! "
set# &
;& '
}( )
}		 
}

 ∑
aC:\Users\yk444\Videos\BTECH\DUMMY\Inkwell\InkWell.Notification\Messaging\Events\PostLikedEvent.cs
	namespace 	
InkWell
 
. 
Shared 
. 
Events 
{ 
public 

class 
PostLikedEvent 
{ 
public 
int 
PostId 
{ 
get 
;  
set! $
;$ %
}& '
public 
int 
PostAuthorId 
{  !
get" %
;% &
set' *
;* +
}, -
public 
int 
ActorId 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
	ActorName 
{  !
get" %
;% &
set' *
;* +
}, -
}		 
}

 ±
kC:\Users\yk444\Videos\BTECH\DUMMY\Inkwell\InkWell.Notification\Messaging\Events\NewsletterPublishedEvent.cs
	namespace 	
InkWell
 
. 
Shared 
. 
Events 
{ 
public 

class $
NewsletterPublishedEvent )
{ 
public 
string 
Subject 
{ 
get  #
;# $
set% (
;( )
}* +
public 
string 
Body 
{ 
get  
;  !
set" %
;% &
}' (
public 
DateTime 
SentAt 
{  
get! $
;$ %
set& )
;) *
}+ ,
} 
}		 ı
dC:\Users\yk444\Videos\BTECH\DUMMY\Inkwell\InkWell.Notification\Messaging\Events\CommentAddedEvent.cs
	namespace 	
InkWell
 
. 
Shared 
. 
Events 
{ 
public 

class 
CommentAddedEvent "
{ 
public 
int 
PostId 
{ 
get 
;  
set! $
;$ %
}& '
public 
int 
	CommentId 
{ 
get "
;" #
set$ '
;' (
}) *
public 
int 
CommentAuthorId "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 
int 
PostAuthorId 
{  !
get" %
;% &
set' *
;* +
}, -
public		 
int		 
?		 
ParentCommentId		 #
{		$ %
get		& )
;		) *
set		+ .
;		. /
}		0 1
public

 
int

 !
ParentCommentAuthorId

 (
{

) *
get

+ .
;

. /
set

0 3
;

3 4
}

5 6
public 
string 
NotificationType &
{' (
get) ,
;, -
set. 1
;1 2
}3 4
public 
string 
	ActorName 
{  !
get" %
;% &
set' *
;* +
}, -
public 
string 
	PostTitle 
{  !
get" %
;% &
set' *
;* +
}, -
} 
} ÷
kC:\Users\yk444\Videos\BTECH\DUMMY\Inkwell\InkWell.Notification\Messaging\Consumers\PostPublishedConsumer.cs
	namespace 	
InkWell
 
. 
Notification 
. 
	Messaging (
.( )
	Consumers) 2
{ 
public		 

class		 !
PostPublishedConsumer		 &
:		' (
	IConsumer		) 2
<		2 3
PostPublishedEvent		3 E
>		E F
{

 
private 
readonly  
INotificationService - 
_notificationService. B
;B C
public !
PostPublishedConsumer $
($ % 
INotificationService% 9
notificationService: M
)M N
{ 	 
_notificationService  
=! "
notificationService# 6
;6 7
} 	
public 
async 
Task 
Consume !
(! "
ConsumeContext" 0
<0 1
PostPublishedEvent1 C
>C D
contextE L
)L M
{ 	
var 
message 
= 
context !
.! "
Message" )
;) *
await  
_notificationService &
.& '
HandlePostPublished' :
(: ;
message; B
.B C
PostIdC I
,I J
messageK R
.R S
TitleS X
,X Y
messageZ a
.a b
AuthorIdb j
)j k
;k l
} 	
} 
} £
gC:\Users\yk444\Videos\BTECH\DUMMY\Inkwell\InkWell.Notification\Messaging\Consumers\PostLikedConsumer.cs
	namespace 	
InkWell
 
. 
Notification 
. 
	Messaging (
.( )
	Consumers) 2
{ 
public 

class 
PostLikedConsumer "
:# $
	IConsumer% .
<. /
PostLikedEvent/ =
>= >
{ 
private		 
readonly		  
INotificationService		 - 
_notificationService		. B
;		B C
public 
PostLikedConsumer  
(  ! 
INotificationService! 5
notificationService6 I
)I J
{ 	 
_notificationService  
=! "
notificationService# 6
;6 7
} 	
public 
async 
Task 
Consume !
(! "
ConsumeContext" 0
<0 1
PostLikedEvent1 ?
>? @
contextA H
)H I
{ 	
var 
message 
= 
context !
.! "
Message" )
;) *
await  
_notificationService &
.& ''
HandlePostLikedPersonalized' B
(B C
messageC J
.J K
PostIdK Q
,Q R
messageS Z
.Z [
PostAuthorId[ g
,g h
messagei p
.p q
ActorIdq x
,x y
message	z Å
.
Å Ç
	ActorName
Ç ã
)
ã å
;
å ç
} 	
} 
} ø
qC:\Users\yk444\Videos\BTECH\DUMMY\Inkwell\InkWell.Notification\Messaging\Consumers\NewsletterPublishedConsumer.cs
	namespace 	
InkWell
 
. 
Notification 
. 
	Messaging (
.( )
	Consumers) 2
{ 
public 

class '
NewsletterPublishedConsumer ,
:- .
	IConsumer/ 8
<8 9$
NewsletterPublishedEvent9 Q
>Q R
{ 
private		 
readonly		  
INotificationService		 - 
_notificationService		. B
;		B C
public '
NewsletterPublishedConsumer *
(* + 
INotificationService+ ?
notificationService@ S
)S T
{ 	 
_notificationService  
=! "
notificationService# 6
;6 7
} 	
public 
async 
Task 
Consume !
(! "
ConsumeContext" 0
<0 1$
NewsletterPublishedEvent1 I
>I J
contextK R
)R S
{ 	
var 
message 
= 
context !
.! "
Message" )
;) *
await  
_notificationService &
.& '
Send' +
(+ ,
recipientId 
: 
$num 
, 
actorId 
: 
$num 
, 
type 
: 
$str "
," #
title 
: 
$str )
+* +
message, 3
.3 4
Subject4 ;
,; <
message 
: 
$str `
,` a
	relatedId 
: 
$num 
, 
relatedType 
: 
$str )
) 
; 
} 	
} 
}   ’
jC:\Users\yk444\Videos\BTECH\DUMMY\Inkwell\InkWell.Notification\Messaging\Consumers\CommentAddedConsumer.cs
	namespace 	
InkWell
 
. 
Notification 
. 
	Messaging (
.( )
	Consumers) 2
{ 
public 

class  
CommentAddedConsumer %
:& '
	IConsumer( 1
<1 2
CommentAddedEvent2 C
>C D
{		 
private

 
readonly

  
INotificationService

 - 
_notificationService

. B
;

B C
public  
CommentAddedConsumer #
(# $ 
INotificationService$ 8
notificationService9 L
)L M
{ 	 
_notificationService  
=! "
notificationService# 6
;6 7
} 	
public 
async 
Task 
Consume !
(! "
ConsumeContext" 0
<0 1
CommentAddedEvent1 B
>B C
contextD K
)K L
{ 	
var 
message 
= 
context !
.! "
Message" )
;) *
var 
dto 
= 
new 
CommentAddedDTO )
{ 
PostId 
= 
message  
.  !
PostId! '
,' (
	CommentId 
= 
message #
.# $
	CommentId$ -
,- .
CommentAuthorId 
=  !
message" )
.) *
CommentAuthorId* 9
,9 :
PostAuthorId 
= 
message &
.& '
PostAuthorId' 3
,3 4
ParentCommentId 
=  !
message" )
.) *
ParentCommentId* 9
,9 :!
ParentCommentAuthorId %
=& '
message( /
./ 0!
ParentCommentAuthorId0 E
,E F
NotificationType  
=! "
message# *
.* +
NotificationType+ ;
,; <
	ActorName 
= 
message #
.# $
	ActorName$ -
} 
; 
await!!  
_notificationService!! &
.!!& '
HandleCommentAdded!!' 9
(!!9 :
dto!!: =
)!!= >
;!!> ?
}"" 	
}## 
}$$ é
SC:\Users\yk444\Videos\BTECH\DUMMY\Inkwell\InkWell.Notification\DTOs\PostLikedDTO.cs
	namespace 	
InkWell
 
. 
Notification 
. 
DTOs #
{ 
public		 

class		 
PostLikedDTO		 
{

 
public 
int 
PostId 
{ 
get 
;  
set! $
;$ %
}& '
public 
int 
PostAuthorId 
{  !
get" %
;% &
set' *
;* +
}, -
public 
int 
ActorId 
{ 
get  
;  !
set" %
;% &
}' (
} 
} Á
^C:\Users\yk444\Videos\BTECH\DUMMY\Inkwell\InkWell.Notification\DTOs\NotificationResponseDTO.cs
	namespace 	
InkWell
 
. 
Notification 
. 
DTOs #
{ 
public 

class #
NotificationResponseDTO (
{ 
public 
int 
NotificationId !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 
int 
RecipientId 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 
int 
ActorId 
{ 
get  
;  !
set" %
;% &
}' (
public		 
string		 
Type		 
{		 
get		  
;		  !
set		" %
;		% &
}		' (
public

 
string

 
Title

 
{

 
get

 !
;

! "
set

# &
;

& '
}

( )
public 
string 
Message 
{ 
get  #
;# $
set% (
;( )
}* +
public 
int 
	RelatedId 
{ 
get "
;" #
set$ '
;' (
}) *
public 
string 
RelatedType !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 
bool 
IsRead 
{ 
get  
;  !
set" %
;% &
}' (
public 
DateTime 
	CreatedAt !
{" #
get$ '
;' (
set) ,
;, -
}. /
} 
} ß
QC:\Users\yk444\Videos\BTECH\DUMMY\Inkwell\InkWell.Notification\DTOs\MentionDTO.cs
	namespace 	
InkWell
 
. 
Notification 
. 
DTOs #
{ 
public		 

class		 

MentionDTO		 
{

 
public 
int 
MentionedUserId "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 
int 
ActorId 
{ 
get  
;  !
set" %
;% &
}' (
public 
int 
	CommentId 
{ 
get "
;" #
set$ '
;' (
}) *
public 
int 
PostId 
{ 
get 
;  
set! $
;$ %
}& '
} 
} ˇ
SC:\Users\yk444\Videos\BTECH\DUMMY\Inkwell\InkWell.Notification\DTOs\BroadcastDTO.cs
	namespace 	
InkWell
 
. 
Notification 
. 
DTOs #
{ 
public 

class 
BroadcastDTO 
{ 
public 
string 
? 
Title 
{ 
get "
;" #
set$ '
;' (
}) *
public 
string 
Message 
{ 
get  #
;# $
set% (
;( )
}* +
public 
string 
? 

RoleFilter !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 
List 
< 
int 
> 
RecipientIds %
{& '
get( +
;+ ,
set- 0
;0 1
}2 3
} 
} Ã
VC:\Users\yk444\Videos\BTECH\DUMMY\Inkwell\InkWell.Notification\DTOs\CommentAddedDTO.cs
	namespace 	
InkWell
 
. 
Notification 
. 
DTOs #
{ 
public 

class 
CommentAddedDTO  
{ 
public 
int 
PostId 
{ 
get 
;  
set! $
;$ %
}& '
public 
int 
	CommentId 
{ 
get "
;" #
set$ '
;' (
}) *
public 
int 
CommentAuthorId "
{# $
get% (
;( )
set* -
;- .
}/ 0
public		 
int		 
PostAuthorId		 
{		  !
get		" %
;		% &
set		' *
;		* +
}		, -
public 
int 
? 
ParentCommentId #
{$ %
get& )
;) *
set+ .
;. /
}0 1
public 
int !
ParentCommentAuthorId (
{) *
get+ .
;. /
set0 3
;3 4
}5 6
public 
string 
NotificationType &
{' (
get) ,
;, -
set. 1
;1 2
}3 4
public 
string 
	ActorName 
{  !
get" %
;% &
set' *
;* +
}, -
} 
} ‹l
dC:\Users\yk444\Videos\BTECH\DUMMY\Inkwell\InkWell.Notification\Controllers\NotificationController.cs
	namespace 	
InkWell
 
. 
Notification 
. 
Controllers *
{ 
[ 
ApiController 
] 
[ 
Route 

(
 
$str 
) 
] 
public 

class "
NotificationController '
:( )
ControllerBase* 8
{ 
private  
INotificationService $
notificationService% 8
;8 9
public "
NotificationController %
(% & 
INotificationService& :
service; B
)B C
{ 	
notificationService 
=  !
service" )
;) *
} 	
[ 	
HttpGet	 
( 
$str 
) 
] 
[ 	
	Authorize	 
] 
public 
async 
Task 
< 
IActionResult '
>' (
GetMy) .
(. /
)/ 0
{ 	
string 
idStr 
= 
User 
.  
FindFirstValue  .
(. /

ClaimTypes/ 9
.9 :
NameIdentifier: H
)H I
;I J
int 
recipientId 
= 
int !
.! "
Parse" '
(' (
idStr( -
)- .
;. /
List!! 
<!! #
NotificationResponseDTO!! (
>!!( )
notifications!!* 7
=!!8 9
await"" 
notificationService"" )
."") *
GetByRecipient""* 8
(""8 9
recipientId""9 D
)""D E
;""E F
return## 
Ok## 
(## 
notifications## #
)### $
;##$ %
}$$ 	
[(( 	
HttpGet((	 
((( 
$str(( 
)(( 
](( 
[)) 	
	Authorize))	 
])) 
public** 
async** 
Task** 
<** 
IActionResult** '
>**' (
	GetUnread**) 2
(**2 3
)**3 4
{++ 	
string,, 
idStr,, 
=,, 
User,, 
.,,  
FindFirstValue,,  .
(,,. /

ClaimTypes,,/ 9
.,,9 :
NameIdentifier,,: H
),,H I
;,,I J
int-- 
recipientId-- 
=-- 
int-- !
.--! "
Parse--" '
(--' (
idStr--( -
)--- .
;--. /
List// 
<// #
NotificationResponseDTO// (
>//( )
notifications//* 7
=//8 9
await00 
notificationService00 )
.00) *
	GetUnread00* 3
(003 4
recipientId004 ?
)00? @
;00@ A
return11 
Ok11 
(11 
notifications11 #
)11# $
;11$ %
}22 	
[66 	
HttpGet66	 
(66 
$str66 
)66  
]66  !
[77 	
	Authorize77	 
]77 
public88 
async88 
Task88 
<88 
IActionResult88 '
>88' (
GetUnreadCount88) 7
(887 8
)888 9
{99 	
string:: 
idStr:: 
=:: 
User:: 
.::  
FindFirstValue::  .
(::. /

ClaimTypes::/ 9
.::9 :
NameIdentifier::: H
)::H I
;::I J
int;; 
recipientId;; 
=;; 
int;; !
.;;! "
Parse;;" '
(;;' (
idStr;;( -
);;- .
;;;. /
int== 
count== 
=== 
await== 
notificationService== 1
.==1 2
GetUnreadCount==2 @
(==@ A
recipientId==A L
)==L M
;==M N
return>> 
Ok>> 
(>> 
new>> 
{>> 
count>> !
=>>" #
count>>$ )
}>>* +
)>>+ ,
;>>, -
}?? 	
[CC 	
HttpPutCC	 
(CC 
$strCC 
)CC 
]CC 
[DD 	
	AuthorizeDD	 
]DD 
publicEE 
asyncEE 
TaskEE 
<EE 
IActionResultEE '
>EE' (

MarkAsReadEE) 3
(EE3 4
intEE4 7
idEE8 :
)EE: ;
{FF 	
tryGG 
{HH 
stringII 
idStrII 
=II 
UserII #
.II# $
FindFirstValueII$ 2
(II2 3

ClaimTypesII3 =
.II= >
NameIdentifierII> L
)IIL M
;IIM N
intJJ 
recipientIdJJ 
=JJ  !
intJJ" %
.JJ% &
ParseJJ& +
(JJ+ ,
idStrJJ, 1
)JJ1 2
;JJ2 3
awaitLL 
notificationServiceLL )
.LL) *

MarkAsReadLL* 4
(LL4 5
idLL5 7
,LL7 8
recipientIdLL9 D
)LLD E
;LLE F
returnMM 
OkMM 
(MM 
newMM 
{MM 
messageMM  '
=MM( )
$strMM* H
}MMI J
)MMJ K
;MMK L
}NN 
catchOO 
(OO 
	ExceptionOO 
exOO 
)OO  
{PP 
returnQQ 

BadRequestQQ !
(QQ! "
newQQ" %
{QQ& '
messageQQ( /
=QQ0 1
exQQ2 4
.QQ4 5
MessageQQ5 <
}QQ= >
)QQ> ?
;QQ? @
}RR 
}SS 	
[WW 	
HttpPutWW	 
(WW 
$strWW 
)WW 
]WW 
[XX 	
	AuthorizeXX	 
]XX 
publicYY 
asyncYY 
TaskYY 
<YY 
IActionResultYY '
>YY' (
MarkAllReadYY) 4
(YY4 5
)YY5 6
{ZZ 	
string[[ 
idStr[[ 
=[[ 
User[[ 
.[[  
FindFirstValue[[  .
([[. /

ClaimTypes[[/ 9
.[[9 :
NameIdentifier[[: H
)[[H I
;[[I J
int\\ 
recipientId\\ 
=\\ 
int\\ !
.\\! "
Parse\\" '
(\\' (
idStr\\( -
)\\- .
;\\. /
await^^ 
notificationService^^ %
.^^% &
MarkAllRead^^& 1
(^^1 2
recipientId^^2 =
)^^= >
;^^> ?
return__ 
Ok__ 
(__ 
new__ 
{__ 
message__ #
=__$ %
$str__& I
}__J K
)__K L
;__L M
}`` 	
[dd 	

HttpDeletedd	 
(dd 
$strdd !
)dd! "
]dd" #
[ee 	
	Authorizeee	 
]ee 
publicff 
asyncff 
Taskff 
<ff 
IActionResultff '
>ff' (
Deleteff) /
(ff/ 0
intff0 3
idff4 6
)ff6 7
{gg 	
tryhh 
{ii 
stringjj 
idStrjj 
=jj 
Userjj #
.jj# $
FindFirstValuejj$ 2
(jj2 3

ClaimTypesjj3 =
.jj= >
NameIdentifierjj> L
)jjL M
;jjM N
intkk 
recipientIdkk 
=kk  !
intkk" %
.kk% &
Parsekk& +
(kk+ ,
idStrkk, 1
)kk1 2
;kk2 3
awaitmm 
notificationServicemm )
.mm) *
DeleteNotificationmm* <
(mm< =
idmm= ?
,mm? @
recipientIdmmA L
)mmL M
;mmM N
returnnn 
Oknn 
(nn 
newnn 
{nn 
messagenn  '
=nn( )
$strnn* A
}nnB C
)nnC D
;nnD E
}oo 
catchpp 
(pp 
	Exceptionpp 
expp 
)pp  
{qq 
returnrr 

BadRequestrr !
(rr! "
newrr" %
{rr& '
messagerr( /
=rr0 1
exrr2 4
.rr4 5
Messagerr5 <
}rr= >
)rr> ?
;rr? @
}ss 
}tt 	
[xx 	

HttpDeletexx	 
(xx 
$strxx !
)xx! "
]xx" #
[yy 	
	Authorizeyy	 
]yy 
publiczz 
asynczz 
Taskzz 
<zz 
IActionResultzz '
>zz' (

DeleteReadzz) 3
(zz3 4
)zz4 5
{{{ 	
string|| 
idStr|| 
=|| 
User|| 
.||  
FindFirstValue||  .
(||. /

ClaimTypes||/ 9
.||9 :
NameIdentifier||: H
)||H I
;||I J
int}} 
recipientId}} 
=}} 
int}} !
.}}! "
Parse}}" '
(}}' (
idStr}}( -
)}}- .
;}}. /
await 
notificationService %
.% &

DeleteRead& 0
(0 1
recipientId1 <
)< =
;= >
return
ÄÄ 
Ok
ÄÄ 
(
ÄÄ 
new
ÄÄ 
{
ÄÄ 
message
ÄÄ #
=
ÄÄ$ %
$str
ÄÄ& C
}
ÄÄD E
)
ÄÄE F
;
ÄÄF G
}
ÅÅ 	
[
ÖÖ 	
HttpGet
ÖÖ	 
(
ÖÖ 
$str
ÖÖ 
)
ÖÖ 
]
ÖÖ 
[
ÜÜ 	
	Authorize
ÜÜ	 
(
ÜÜ 
Roles
ÜÜ 
=
ÜÜ 
$str
ÜÜ "
)
ÜÜ" #
]
ÜÜ# $
public
áá 
async
áá 
Task
áá 
<
áá 
IActionResult
áá '
>
áá' (
GetAll
áá) /
(
áá/ 0
)
áá0 1
{
àà 	
List
ââ 
<
ââ %
NotificationResponseDTO
ââ (
>
ââ( )
notifications
ââ* 7
=
ââ8 9
await
ää !
notificationService
ää )
.
ää) *
GetAll
ää* 0
(
ää0 1
)
ää1 2
;
ää2 3
return
ãã 
Ok
ãã 
(
ãã 
notifications
ãã #
)
ãã# $
;
ãã$ %
}
åå 	
[
êê 	
HttpPost
êê	 
(
êê 
$str
êê 
)
êê 
]
êê 
[
ëë 	
	Authorize
ëë	 
(
ëë 
Roles
ëë 
=
ëë 
$str
ëë "
)
ëë" #
]
ëë# $
public
íí 
async
íí 
Task
íí 
<
íí 
IActionResult
íí '
>
íí' (
	Broadcast
íí) 2
(
íí2 3
[
íí3 4
FromBody
íí4 <
]
íí< =
BroadcastDTO
íí> J
dto
ííK N
)
ííN O
{
ìì 	
try
îî 
{
ïï 
await
ññ !
notificationService
ññ )
.
ññ) *
SendBulk
ññ* 2
(
ññ2 3
dto
ññ3 6
)
ññ6 7
;
ññ7 8
return
óó 
Ok
óó 
(
óó 
new
óó 
{
óó 
message
óó  '
=
óó( )
$str
óó* ;
}
óó< =
)
óó= >
;
óó> ?
}
òò 
catch
ôô 
(
ôô 
	Exception
ôô 
ex
ôô 
)
ôô  
{
öö 
return
õõ 

BadRequest
õõ !
(
õõ! "
new
õõ" %
{
õõ& '
message
õõ( /
=
õõ0 1
ex
õõ2 4
.
õõ4 5
Message
õõ5 <
}
õõ= >
)
õõ> ?
;
õõ? @
}
úú 
}
ùù 	
[
°° 	
HttpGet
°°	 
(
°° 
$str
°° 
)
°° 
]
°° 
[
¢¢ 	
	Authorize
¢¢	 
(
¢¢ 
Roles
¢¢ 
=
¢¢ 
$str
¢¢ "
)
¢¢" #
]
¢¢# $
public
££ 
async
££ 
Task
££ 
<
££ 
IActionResult
££ '
>
££' (
	GetByType
££) 2
(
££2 3
[
££3 4
	FromQuery
££4 =
]
££= >
string
££? E
type
££F J
)
££J K
{
§§ 	
List
•• 
<
•• %
NotificationResponseDTO
•• (
>
••( )
notifications
••* 7
=
••8 9
await
¶¶ !
notificationService
¶¶ )
.
¶¶) *
	GetByType
¶¶* 3
(
¶¶3 4
type
¶¶4 8
)
¶¶8 9
;
¶¶9 :
return
ßß 
Ok
ßß 
(
ßß 
notifications
ßß #
)
ßß# $
;
ßß$ %
}
®® 	
[
¨¨ 	
HttpGet
¨¨	 
(
¨¨ 
$str
¨¨ )
)
¨¨) *
]
¨¨* +
[
≠≠ 	
	Authorize
≠≠	 
(
≠≠ 
Roles
≠≠ 
=
≠≠ 
$str
≠≠ "
)
≠≠" #
]
≠≠# $
public
ÆÆ 
async
ÆÆ 
Task
ÆÆ 
<
ÆÆ 
IActionResult
ÆÆ '
>
ÆÆ' (
GetByRelatedId
ÆÆ) 7
(
ÆÆ7 8
int
ÆÆ8 ;
	relatedId
ÆÆ< E
)
ÆÆE F
{
ØØ 	
List
∞∞ 
<
∞∞ %
NotificationResponseDTO
∞∞ (
>
∞∞( )
notifications
∞∞* 7
=
∞∞8 9
await
±± !
notificationService
±± )
.
±±) *
GetByRelatedId
±±* 8
(
±±8 9
	relatedId
±±9 B
)
±±B C
;
±±C D
return
≤≤ 
Ok
≤≤ 
(
≤≤ 
notifications
≤≤ #
)
≤≤# $
;
≤≤$ %
}
≥≥ 	
}
¥¥ 
}µµ ò
_C:\Users\yk444\Videos\BTECH\DUMMY\Inkwell\InkWell.Notification\Context\NotificationDbContext.cs
	namespace 	
InkWell
 
. 
Notification 
. 
Context &
{ 
public 

class !
NotificationDbContext &
:' (
	DbContext) 2
{ 
public !
NotificationDbContext $
($ %
DbContextOptions% 5
<5 6!
NotificationDbContext6 K
>K L
optionsM T
)T U
:V W
baseX \
(\ ]
options] d
)d e
{		 	
}

 	
public 
DbSet 
< 
NotificationModel &
>& '
Notifications( 5
{6 7
get8 ;
;; <
set= @
;@ A
}B C
} 
} 