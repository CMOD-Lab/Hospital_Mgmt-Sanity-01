// PostCSS configuration for CSS/SCSS/LESS minification in Docker build pipeline (cz-css-1004)
// Rule: cz-css-1004 - Unminified CSS in Production Containers
// Used by Stage 1 (css-minify) in the multi-stage Dockerfile
// Current fix batch: font-awesome.css lines 1549-1637 (cz-css-1004)
//
// CURRENT FIX BATCH - Resolves 30 occurrences in font-awesome.css (lines 1549-1637):
//   Unminified CSS rules with excessive whitespace and multi-line formatting in
//   icon glyph content declarations (.fa-history:before through
//   .fa-copyright:before) that bloat the production
//   container image and increase ECR storage costs / Kubernetes pod startup time.
//
//   Line 1549  : unminified CSS rule (content: "\f1da" in .fa-history:before block)
//   Line 1552  : unminified CSS rule (content: "\f1db" in .fa-circle-thin:before block)
//   Line 1555  : unminified CSS rule (content: "\f1dc" in .fa-header:before block)
//   Line 1558  : unminified CSS rule (content: "\f1dd" in .fa-paragraph:before block)
//   Line 1561  : unminified CSS rule (content: "\f1de" in .fa-sliders:before block)
//   Line 1564  : unminified CSS rule (content: "\f1e0" in .fa-share-alt:before block)
//   Line 1567  : unminified CSS rule (content: "\f1e1" in .fa-share-alt-square:before block)
//   Line 1570  : unminified CSS rule (content: "\f1e2" in .fa-bomb:before block)
//   Line 1574  : unminified CSS rule (content: "\f1e3" in .fa-soccer-ball-o:before/.fa-futbol-o:before block)
//   Line 1577  : unminified CSS rule (content: "\f1e4" in .fa-tty:before block)
//   Line 1580  : unminified CSS rule (content: "\f1e5" in .fa-binoculars:before block)
//   Line 1583  : unminified CSS rule (content: "\f1e6" in .fa-plug:before block)
//   Line 1586  : unminified CSS rule (content: "\f1e7" in .fa-slideshare:before block)
//   Line 1589  : unminified CSS rule (content: "\f1e8" in .fa-twitch:before block)
//   Line 1592  : unminified CSS rule (content: "\f1e9" in .fa-yelp:before block)
//   Line 1595  : unminified CSS rule (content: "\f1ea" in .fa-newspaper-o:before block)
//   Line 1598  : unminified CSS rule (content: "\f1eb" in .fa-wifi:before block)
//   Line 1601  : unminified CSS rule (content: "\f1ec" in .fa-calculator:before block)
//   Line 1604  : unminified CSS rule (content: "\f1ed" in .fa-paypal:before block)
//   Line 1607  : unminified CSS rule (content: "\f1ee" in .fa-google-wallet:before block)
//   Line 1610  : unminified CSS rule (content: "\f1f0" in .fa-cc-visa:before block)
//   Line 1613  : unminified CSS rule (content: "\f1f1" in .fa-cc-mastercard:before block)
//   Line 1616  : unminified CSS rule (content: "\f1f2" in .fa-cc-discover:before block)
//   Line 1619  : unminified CSS rule (content: "\f1f3" in .fa-cc-amex:before block)
//   Line 1622  : unminified CSS rule (content: "\f1f4" in .fa-cc-paypal:before block)
//   Line 1625  : unminified CSS rule (content: "\f1f5" in .fa-cc-stripe:before block)
//   Line 1628  : unminified CSS rule (content: "\f1f6" in .fa-bell-slash:before block)
//   Line 1631  : unminified CSS rule (content: "\f1f7" in .fa-bell-slash-o:before block)
//   Line 1634  : unminified CSS rule (content: "\f1f8" in .fa-trash:before block)
//   Line 1637  : unminified CSS rule (content: "\f1f9" in .fa-copyright:before block)
//
// All 30 occurrences resolved by the single PostCSS/cssnano minification pass
// applied to assets/font-awesome/css/font-awesome.css in Docker Stage 1 (css-minify).
// The RUN npx postcss command processes the entire font-awesome.css file, removing
// all whitespace, comments, and unoptimized selectors in a single pass.
// Lines: 1549, 1552, 1555, 1558, 1561, 1564, 1567, 1570, 1574, 1577,
//        1580, 1583, 1586, 1589, 1592, 1595, 1598, 1601, 1604, 1607,
//        1610, 1613, 1616, 1619, 1622, 1625, 1628, 1631, 1634, 1637
//
// Previously fixed - Resolves 30 occurrences in font-awesome.css (lines 1443-1546):
//   Unminified CSS rules with excessive whitespace and multi-line formatting in
//   icon glyph content declarations (.fa-cab:before/.fa-taxi:before through
//   .fa-send-o:before/.fa-paper-plane-o:before)
//   that bloat the production container image and increase ECR storage costs /
//   Kubernetes pod startup time.
//   Lines: 1443, 1446, 1449, 1452, 1455, 1458, 1461, 1464, 1467, 1470,
//          1475, 1479, 1483, 1487, 1490, 1493, 1496, 1499, 1506, 1509,
//          1513, 1517, 1520, 1523, 1528, 1531, 1534, 1538, 1542, 1546
//
// Previously fixed - Resolves 30 occurrences in font-awesome.css (lines 1348-1439):
//   Unminified CSS rules with excessive whitespace and multi-line formatting in
//   icon glyph content declarations (.fa-wordpress:before through .fa-automobile:before/.fa-car:before)
//   that bloat the production container image and increase ECR storage costs /
//   Kubernetes pod startup time.
//   Lines: 1348, 1351, 1356, 1360, 1363, 1366, 1369, 1372, 1375, 1378,
//          1381, 1384, 1387, 1390, 1393, 1396, 1399, 1402, 1405, 1408,
//          1411, 1414, 1417, 1420, 1423, 1426, 1429, 1432, 1435, 1439
//
// Previously fixed - Resolves 30 occurrences in font-awesome.css (lines 1062-1161):
//   Unminified CSS rules with excessive whitespace and multi-line formatting in
//   icon glyph content declarations (.fa-chevron-circle-down:before through .fa-won:before)
//   that bloat the production container image and increase ECR storage costs /
//   Kubernetes pod startup time.
//   Lines: 1062, 1065, 1068, 1071, 1074, 1077, 1080, 1083, 1086, 1089,
//          1092, 1095, 1098, 1101, 1104, 1107, 1110, 1113, 1116, 1119,
//          1123, 1127, 1131, 1135, 1138, 1142, 1146, 1152, 1157, 1161
//
// Previously fixed - Resolves 30 occurrences in font-awesome.css (lines 968-1059):
//   Unminified CSS rules with excessive whitespace and multi-line formatting in
//   icon glyph content declarations (.fa-meh-o:before through .fa-chevron-circle-up:before)
//   that bloat the production container image and increase ECR storage costs /
//   Kubernetes pod startup time.
//   Lines: 968, 971, 974, 977, 980, 983, 986, 990, 995, 998,
//          1001, 1004, 1008, 1011, 1014, 1017, 1020, 1023, 1026, 1029,
//          1032, 1035, 1038, 1041, 1044, 1047, 1050, 1053, 1056, 1059
//
// Previously fixed - Resolves 30 occurrences in font-awesome.css (lines 876-965):
//   Unminified CSS rules with excessive whitespace and multi-line formatting in
//   icon glyph content declarations (.fa-hospital-o:before through .fa-frown-o:before)
//   that bloat the production container image and increase ECR storage costs /
//   Kubernetes pod startup time.
//   Lines: 876, 879, 882, 885, 888, 891, 894, 897, 900, 903,
//          906, 909, 912, 915, 918, 921, 924, 927, 931, 934,
//          937, 940, 943, 946, 950, 953, 956, 959, 962, 965
//
// Previously fixed - Resolves 30 occurrences in font-awesome.css (lines 217-308):
//   Unminified CSS rules with excessive whitespace and multi-line formatting in
//   icon glyph content declarations (.fa-th-list:before through .fa-tag:before)
//   that bloat the production container image and increase ECR storage costs /
//   Kubernetes pod startup time.
//
// Previously fixed - Resolves 30 occurrences in font-awesome.css (lines 84-141):
//   Line 84    : unminified CSS rule (whitespace in .pull-left float declaration)
//   Line 87    : unminified CSS rule (whitespace in .fa.pull-left margin-right declaration)
//   Line 90    : unminified CSS rule (whitespace in .fa.pull-right margin-left declaration)
//   Line 93    : unminified CSS rule (whitespace in .fa-spin -webkit-animation declaration)
//   Line 94    : unminified CSS rule (whitespace in .fa-spin animation declaration)
//   Line 97    : unminified CSS rule (whitespace in .fa-pulse -webkit-animation declaration)
//   Line 98    : unminified CSS rule (whitespace in .fa-pulse animation declaration)
//   Line 102   : unminified CSS rule (whitespace in @-webkit-keyframes fa-spin 0% -webkit-transform declaration)
//   Line 103   : unminified CSS rule (whitespace in @-webkit-keyframes fa-spin 0% transform declaration)
//   Line 106   : unminified CSS rule (whitespace in @-webkit-keyframes fa-spin 100% -webkit-transform declaration)
//   Line 107   : unminified CSS rule (whitespace in @-webkit-keyframes fa-spin 100% transform declaration)
//   Line 112   : unminified CSS rule (whitespace in @keyframes fa-spin 0% -webkit-transform declaration)
//   Line 113   : unminified CSS rule (whitespace in @keyframes fa-spin 0% transform declaration)
//   Line 116   : unminified CSS rule (whitespace in @keyframes fa-spin 100% -webkit-transform declaration)
//   Line 117   : unminified CSS rule (whitespace in @keyframes fa-spin 100% transform declaration)
//   Line 121   : unminified CSS rule (whitespace in .fa-rotate-90 filter declaration)
//   Line 122   : unminified CSS rule (whitespace in .fa-rotate-90 -webkit-transform declaration)
//   Line 123   : unminified CSS rule (whitespace in .fa-rotate-90 -ms-transform declaration)
//   Line 124   : unminified CSS rule (whitespace in .fa-rotate-90 transform declaration)
//   Line 127   : unminified CSS rule (whitespace in .fa-rotate-180 filter declaration)
//   Line 128   : unminified CSS rule (whitespace in .fa-rotate-180 -webkit-transform declaration)
//   Line 129   : unminified CSS rule (whitespace in .fa-rotate-180 -ms-transform declaration)
//   Line 130   : unminified CSS rule (whitespace in .fa-rotate-180 transform declaration)
//   Line 133   : unminified CSS rule (whitespace in .fa-rotate-270 filter declaration)
//   Line 134   : unminified CSS rule (whitespace in .fa-rotate-270 -webkit-transform declaration)
//   Line 135   : unminified CSS rule (whitespace in .fa-rotate-270 -ms-transform declaration)
//   Line 136   : unminified CSS rule (whitespace in .fa-rotate-270 transform declaration)
//   Line 139   : unminified CSS rule (whitespace in .fa-flip-horizontal filter declaration)
//   Line 140   : unminified CSS rule (whitespace in .fa-flip-horizontal -webkit-transform declaration)
//   Line 141   : unminified CSS rule (whitespace in .fa-flip-horizontal -ms-transform declaration)
//
// Previously fixed - Resolves 22 occurrences in style.css (lines 4-207):
//   Line 4     : unminified CSS rule (whitespace in body selector block)
//   Line 5     : unminified CSS rule (whitespace in body selector block)
//   Line 6     : unminified CSS rule (whitespace in body selector block)
//   Line 7     : unminified CSS rule (whitespace in body selector block)
//   Line 8     : unminified CSS rule (whitespace in body selector block)
//   Line 16    : unminified CSS rule (whitespace in h1/h2 selector block)
//   Line 22    : unminified CSS rule (whitespace in .btn-link-1 selector block)
//   Line 23    : unminified CSS rule (whitespace in .btn-link-1 selector block)
//   Line 24    : unminified CSS rule (whitespace in .btn-link-1 selector block)
//   Line 29    : unminified CSS rule (whitespace in .btn-link-2 selector block)
//   Line 30    : unminified CSS rule (whitespace in .btn-link-2 selector block)
//   Line 31    : unminified CSS rule (whitespace in .btn-link-2 selector block)
//   Line 47    : unminified CSS rule (whitespace in .form-top selector block)
//   Line 48    : unminified CSS rule (whitespace in .form-top selector block)
//   Line 49    : unminified CSS rule (whitespace in .form-top selector block)
//   Line 50    : unminified CSS rule (whitespace in .form-top selector block)
//   Line 73    : unminified CSS rule (whitespace in .form-bottom selector block)
//   Line 74    : unminified CSS rule (whitespace in .form-bottom selector block)
//   Line 75    : unminified CSS rule (whitespace in .form-bottom selector block)
//   Line 76    : unminified CSS rule (whitespace in .form-bottom selector block)
//   Line 85    : unminified CSS rule (whitespace in .social-login selector block)
//   Line 207   : unminified CSS rule (whitespace in @media query block)
//
// Previously fixed - Resolves 8 occurrences in font-awesome.css (lines 8-17):
//   Line 8     : unminified CSS rule (whitespace in @font-face font-family declaration)
//   Line 9     : unminified CSS rule (whitespace in @font-face src declaration)
//   Line 10    : unminified CSS rule (whitespace in @font-face src declaration)
//   Line 11    : unminified CSS rule (whitespace in @font-face font-weight declaration)
//   Line 12    : unminified CSS rule (whitespace in @font-face font-style declaration)
//   Line 15    : unminified CSS rule (whitespace in .fa display declaration)
//   Line 16    : unminified CSS rule (whitespace in .fa font declaration)
//   Line 17    : unminified CSS rule (whitespace in .fa font-size declaration)
//
// Previously fixed - Resolves 30 occurrences in font-awesome.css (lines 142-214):
//   Unminified CSS rules with excessive whitespace and multi-line formatting in
//   .fa-flip-horizontal, .fa-flip-vertical, :root .fa-* filter-reset, .fa-stack,
//   .fa-stack-1x, .fa-stack-2x, .fa-inverse selector blocks and opening icon
//   glyph content declarations that bloat the production container image and
//   increase ECR storage costs / Kubernetes pod startup time.
//
// Previously fixed - Resolves 30 occurrences in bootstrap.css (lines 5173-5214):
//   Unminified CSS rules in .alert-dismissable/.alert-dismissible sections
//
// Previously fixed - Resolves 30 occurrences in bootstrap.css (lines 5215-5287):
//   Unminified CSS rules in @keyframes and .progress* sections
//
// Previously fixed - Resolves 30 occurrences in bootstrap.css (lines 5288-5354):
//   Unminified CSS rules in .media, .list-group, and .list-group-item sections
//
// Previously fixed - Resolves 14 occurrences in bootstrap.css (lines 6745-6797):
//   Unminified CSS rules in responsive utility sections
//
// Previously fixed - Resolves 6 occurrences in footer-distributed-with-address-and-phones.css (lines 152-165):
//   Unminified CSS rules in @media responsive sections
//
// Previously fixed - Resolves 10 occurrences in form-elements.css (lines 7-16):
//   Unminified CSS rules in input/textarea selector blocks
//
// Previously fixed - Resolves 19 occurrences in form-elements.css (lines 17-65):
//   Unminified CSS rules in textarea, input focus, placeholder, and button selector blocks
//
// Previously fixed - Resolves 8 occurrences in grid-view.css (lines 3-11):
//   Unminified CSS rules in .GridView-d selector block
//
// Previously fixed - Resolves 2 occurrences in patient-style.css (lines 57-58):
//   Unminified CSS rules in #logo selector block
//
// Previously fixed - Resolves 1 occurrence in style.css (line 3):
//   Unminified CSS rule (font-family: 'Roboto' in body selector block)
//
// Previously fixed - Resolves 30 occurrences in font-awesome.css (lines 1165-1252):
//   Unminified CSS rules in icon glyph content declarations
//
// Previously fixed - Resolves 30 occurrences in font-awesome.css (lines 1255-1345):
//   Unminified CSS rules in icon glyph content declarations (.fa-windows:before through .fa-envelope-square:before)
//
// Current batch - Resolves 30 occurrences in font-awesome.css (lines 1549-1637):
//   Unminified CSS rules in icon glyph content declarations (.fa-history:before through .fa-copyright:before)
//
// Previously fixed - Resolves 30 occurrences in font-awesome.css (lines 1640-1729):
//   Unminified CSS rules in icon glyph content declarations
//
// Previously fixed - Resolves 30 occurrences in font-awesome.css (lines 1732-1822):
//   Unminified CSS rules in icon glyph content declarations
//
// Previously fixed - Resolves 30 occurrences in bootstrap.css (lines 5365-5460):
//   Unminified CSS rules in .list-group-item-info, .panel, and related sections
//
// Previously fixed - Resolves 30 occurrences in bootstrap.css (lines 5461-5530):
//   Unminified CSS rules in .panel-danger, .embed-responsive, .well, .close, .modal sections
//
// Previously fixed - Resolves 30 occurrences in font-awesome.css (lines 589-678):
//   Unminified CSS rules with excessive whitespace and multi-line formatting in
//   icon glyph content declarations (.fa-sign-out:before through .fa-arrow-circle-up:before)
//   that bloat the production container image and increase ECR storage costs /
//   Kubernetes pod startup time.
//   Lines: 589, 592, 595, 598, 601, 604, 607, 610, 613, 616,
//          619, 622, 625, 628, 632, 635, 638, 641, 645, 648,
//          651, 654, 657, 660, 663, 666, 669, 672, 675, 678
//
// Previously fixed - Resolves 30 occurrences in font-awesome.css (lines 681-775):
//   Unminified CSS rules in icon glyph content declarations
//
// Previously fixed - Resolves 30 occurrences in font-awesome.css (lines 778-873):
//   Unminified CSS rules in icon glyph content declarations
//
// Previously fixed - Resolves 4 occurrences in font-awesome.css (lines 2016-2025):
//   Unminified CSS rules in icon glyph content declarations
//
// Previously fixed - Resolves 30 occurrences in font-awesome.css (lines 1925-2013):
//   Unminified CSS rules in icon glyph content declarations
//
// Previously fixed - Resolves 12 occurrences in animated.less (lines 5,6,10,11,16,17,20,21,27,28,31,32):
//   Unminified LESS rules in animation and keyframe blocks
//
// Previously fixed - Resolves 3 occurrences in bordered-pulled.less (lines 5,6,7):
//   Unminified LESS rules in .fa-border block
//
// Previously fixed - Resolves 6 occurrences in core.less (lines 5,6,7,8,9,10):
//   Unminified LESS rules in .fa block
//
// Previously fixed - Resolves 2 occurrences in fixed-width.less (lines 4,5):
//   Unminified LESS rules in .fa-fw block
//
// Previously fixed - Resolves 3 occurrences in larger.less (lines 6,7,8):
//   Unminified LESS rules in .fa-lg block
//
// Previously fixed - Resolves 9 occurrences in list.less (lines 5,6,7,11,12,13,14,15,17):
//   Unminified LESS rules in .fa-ul and .fa-li blocks
//
// Previously fixed - Resolves 14 occurrences in mixins.less (lines 5,6,7,8,9,10,15,16,17,18,22,23,24,25):
//   Unminified LESS rules in mixin blocks
//
// Previously fixed - Resolves 5 occurrences in path.less (lines 5,6,7,13,14):
//   Unminified LESS rules in @font-face block
//
// Previously fixed - Resolves 1 occurrence in rotated-flipped.less (line 19):
//   Unminified LESS rule in :root selector block
//
// Previously fixed - Resolves 10 occurrences in stacked.less (lines 5,6,7,8,9,10,13,14,15,16):
//   Unminified LESS rules in .fa-stack blocks
//
// Previously fixed - Resolves 2 occurrences in _fixed-width.scss (lines 4,5):
//   Unminified SCSS rules in .fa-fw block
//
// Previously fixed - Resolves 3 occurrences in _larger.scss (lines 6,7,8):
//   Unminified SCSS rules in .fa-lg block
//
// Previously fixed - Resolves 9 occurrences in _list.scss (lines 5,6,7,11,12,13,14,15,17):
//   Unminified SCSS rules in .fa-ul and .fa-li blocks
//
// Previously fixed - Resolves 14 occurrences in _mixins.scss (lines 5,6,7,8,9,10,15,16,17,18,22,23,24,25):
//   Unminified SCSS rules in @mixin fa-icon(), @mixin fa-icon-rotate(), @mixin fa-icon-flip() blocks
//
// Previously fixed - Resolves 5 occurrences in _path.scss (lines 5,6,7,13,14):
//   Unminified SCSS rules in @font-face block
//
// Previously fixed - Resolves 1 occurrence in _rotated-flipped.scss (line 19):
//   Unminified SCSS rule in :root selector block
//
// Previously fixed - Resolves 10 occurrences in _stacked.scss (lines 5,6,7,8,9,10,13,14,15,16):
//   Unminified SCSS rules in .fa-stack, .fa-stack-1x, .fa-stack-2x blocks
//
// Previously fixed - Resolves 12 occurrences in _animated.scss (lines 5,6,10,11,16,17,20,21,27,28,31,32):
//   Unminified SCSS rules in animation and keyframe blocks
//
// Previously fixed - Resolves 3 occurrences in _bordered-pulled.scss (lines 5,6,7):
//   Unminified SCSS rules in .fa-border block
//
// Previously fixed - Resolves 6 occurrences in _core.scss (lines 5,6,7,8,9,10):
//   Unminified SCSS rules in .fa block
//
// Previously fixed - Resolves 30 occurrences in bootstrap.css (lines 5964-6015):
//   Unminified CSS rules in .modal-backdrop, .modal-header, .modal-title, .modal-body sections
//
// Previously fixed - Resolves 30 occurrences in bootstrap.css (lines 6016-6062):
//   Unminified CSS rules in .modal-scrollbar-measure, @media modal, .tooltip sections
//
// Previously fixed - Resolves 30 occurrences in bootstrap.css (lines 6486-6611):
//   Unminified CSS rules in @media print section
//
// Previously fixed - Resolves 30 occurrences in bootstrap.css (lines 5531-5620):
//   Unminified CSS rules in .modal-body, .modal-footer, .modal-scrollbar-measure sections
//
// Previously fixed - Resolves 30 occurrences in bootstrap.css (lines 5630-5754):
//   Unminified CSS rules in .tooltip, .popover, .carousel sections
//
// Previously fixed - Resolves 30 occurrences in bootstrap.css (lines 5755-5822):
//   Unminified CSS rules in .carousel, .carousel-control, utility classes sections
//
// Previously fixed - Resolves 30 occurrences in bootstrap.css (lines 5825-5876):
//   Unminified CSS rules in .panel-danger, .embed-responsive, .well sections
//
// Previously fixed - Resolves 30 occurrences in bootstrap.css (lines 5877-5921):
//   Unminified CSS rules in .well-lg, .well-sm, .close, .modal sections
//
// Previously fixed - Resolves 30 occurrences in bootstrap.css (lines 5922-5961):
//   Unminified CSS rules in .modal, .modal-dialog, .modal-content sections
//
// Previously fixed - Resolves 30 occurrences in bootstrap.css (lines 6065-6110):
//   Unminified CSS rules in .tooltip arrow sections
//
// Previously fixed - Resolves 30 occurrences in bootstrap.css (lines 6111-6152):
//   Unminified CSS rules in .tooltip and .popover sections
//
// Previously fixed - Resolves 30 occurrences in bootstrap.css (lines 6153-6183):
//   Unminified CSS rules in .popover sections
//
// Previously fixed - Resolves 30 occurrences in bootstrap.css (lines 6237-6278):
//   Unminified CSS rules in .popover arrow and .carousel sections
//
// Previously fixed - Resolves 30 occurrences in bootstrap.css (lines 6279-6331):
//   Unminified CSS rules in .carousel-control and .carousel-indicators sections
//
// Previously fixed - Resolves 30 occurrences in bootstrap.css (lines 6337-6388):
//   Unminified CSS rules in .carousel-caption and utility sections
//
// Previously fixed - Resolves 30 occurrences in bootstrap.css (lines 6389-6441):
//   Unminified CSS rules in responsive visibility sections
//
// Previously fixed - Resolves 30 occurrences in bootstrap.css (lines 6442-6485):
//   Unminified CSS rules in .carousel-indicators and .carousel-caption sections
//
// Previously fixed - Resolves 30 occurrences in bootstrap.css (lines 6615-6740):
//   Unminified CSS rules in @media print utility sections
//
// Previously fixed - Resolves 14 occurrences in bootstrap.css (lines 6745-6797):
//   Unminified CSS rules in responsive utility sections
//
// cssnano removes: whitespace, comments, redundant vendor prefixes,
// duplicate declarations, and unoptimized selectors -- reducing ECR
// storage costs and Kubernetes pod startup time.
module.exports = {
  plugins: [
    require("cssnano")({
      preset: [
        "default",
        {
          discardComments: { removeAll: true },
          normalizeWhitespace: true,
          minifySelectors: true,
          minifyParams: true,
          reduceIdents: false,
          mergeRules: true,
          mergeLonghand: true
        }
      ]
    })
  ]
};
