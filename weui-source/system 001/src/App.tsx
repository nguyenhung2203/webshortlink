/**
 * @license
 * SPDX-License-Identifier: Apache-2.0
 */

import { useState, useEffect } from 'react';
import { 
  Bolt, 
  CheckCircle2, 
  Moon, 
  Sun,
  Search, 
  User, 
  Lock, 
  Eye, 
  ChevronDown, 
  ChevronLeft,
  ChevronRight,
  ArrowUp,
  ArrowDown,
  ArrowUpDown,
  Home, 
  Settings, 
  List, 
  Trash2, 
  Check, 
  Loader2, 
  Bell,
  ExternalLink,
  Github,
  HelpCircle,
  FileText,
  CloudUpload,
  Calendar,
  ArrowRight,
  Plus,
  ArrowLeft,
  Share2,
  Play,
  AlertTriangle,
  Download,
  Folder,
  Briefcase,
  Layout,
  MessageSquare,
  Coins,
  AlertCircle,
  Info,
  X,
  Maximize2,
  Minimize2,
  Square,
  Palette,
  Type,
  Layers,
  Menu,
  Activity,
  Users,
  RefreshCw,
  BarChart3,
  PieChart as PieChartIcon,
  TrendingUp,
  Filter,
  MoreHorizontal,
  XCircle,
  Globe,
  Sliders,
  EyeOff,
  Settings2,
  ChevronRight as ChevronRightIcon
} from 'lucide-react';
import { motion, AnimatePresence } from 'motion/react';
import { 
  LineChart, 
  Line, 
  XAxis, 
  YAxis, 
  CartesianGrid, 
  Tooltip, 
  ResponsiveContainer,
  BarChart,
  Bar,
  Cell,
  PieChart,
  Pie
} from 'recharts';

export default function App() {
  const [darkMode, setDarkMode] = useState(false);
  const [activePage, setActivePage] = useState('primitives');
  const [activeTab, setActiveTab] = useState('general');
  const [toggleOn, setToggleOn] = useState(true);
  const [radioValue, setRadioValue] = useState('A');
  const [checkboxChecked, setCheckboxChecked] = useState(true);
  const [isPasswordVisible, setIsPasswordVisible] = useState(false);
  const [sliderValue, setSliderValue] = useState(40);
  const [isDrawerOpen, setIsDrawerOpen] = useState(false);
  const [drawerCampaignName, setDrawerCampaignName] = useState('Campaign_A');
  const [drawerMaxSpend, setDrawerMaxSpend] = useState(40);
  const [drawerAutoOptimize, setDrawerAutoOptimize] = useState(true);
  const [activeStep, setActiveStep] = useState(0);
  const [expandedNodes, setExpandedNodes] = useState<string[]>(['bm-001']);
  const [activeSideNav, setActiveSideNav] = useState('dashboard');
  const [activeTokenNav, setActiveTokenNav] = useState('foundation-surfaces');
  const [removableTags, setRemovableTags] = useState([1, 2, 3, 4]);
  const [removableFilters, setRemovableFilters] = useState(['Status: Active', 'Region: VN', 'Budget: >$500', 'Platform: FB']);
  const [removableUsers, setRemovableUsers] = useState([
    { id: 1, name: 'Nguyễn Văn A', role: 'Admin' },
    { id: 2, name: 'Trần Thị B', role: 'Editor' }
  ]);
  const [patternsToggles, setPatternsToggles] = useState({
    autoDownload: true,
    showNotifications: false,
    syncBackground: true,
    loginProtection: true
  });
  const [selectedOption, setSelectedOption] = useState('option1');
  const [expandedPanels, setExpandedPanels] = useState<string[]>(['panel1']);
  const [isPopoverOpen, setIsPopoverOpen] = useState(false);
  const initialTableData = [
    { id: '1', name: 'TKQC_Campaign_A', status: 'ACTIVE', spend: '$1,234.56', updated: '22/03/2026' },
    { id: '2', name: 'TKQC_Campaign_B', status: 'PAUSED', spend: '$567.89', updated: '21/03/2026' },
    { id: '3', name: 'TKQC_Retarget_01', status: 'ACTIVE', spend: '$2,345.00', updated: '22/03/2026' },
    { id: '4', name: 'TKQC_Lookalike_VN', status: 'ERROR', spend: '$0.00', updated: '20/03/2026' },
    { id: '5', name: 'TKQC_Brand_Winter', status: 'ACTIVE', spend: '$890.12', updated: '22/03/2026' },
    { id: '6', name: 'TKQC_DPA_Catalog', status: 'COMPLETED', spend: '$3,456.78', updated: '19/03/2026' },
  ];
  const [tableData, setTableData] = useState(initialTableData);
  const [sortConfig, setSortConfig] = useState<{ key: string; direction: 'asc' | 'desc' | null }>({ key: '', direction: null });

  const handleSort = (key: string) => {
    let direction: 'asc' | 'desc' | null = 'asc';
    if (sortConfig.key === key && sortConfig.direction === 'asc') {
      direction = 'desc';
    } else if (sortConfig.key === key && sortConfig.direction === 'desc') {
      direction = null;
    }

    setSortConfig({ key, direction });

    if (direction === null) {
      setTableData(initialTableData);
      return;
    }

    const sortedData = [...tableData].sort((a: any, b: any) => {
      let valA = a[key];
      let valB = b[key];

      // Special handling for currency
      if (key === 'spend') {
        valA = parseFloat(valA.replace(/[$,]/g, ''));
        valB = parseFloat(valB.replace(/[$,]/g, ''));
      }
      // Special handling for date
      if (key === 'updated') {
        const [dayA, monthA, yearA] = valA.split('/');
        const [dayB, monthB, yearB] = valB.split('/');
        valA = new Date(`${yearA}-${monthA}-${dayA}`).getTime();
        valB = new Date(`${yearB}-${monthB}-${dayB}`).getTime();
      }
      // Special handling for ID (numeric)
      if (key === 'id') {
        valA = parseInt(valA);
        valB = parseInt(valB);
      }

      if (valA < valB) return direction === 'asc' ? -1 : 1;
      if (valA > valB) return direction === 'asc' ? 1 : -1;
      return 0;
    });

    setTableData(sortedData);
  };

  const getSortIcon = (key: string) => {
    if (sortConfig.key !== key || sortConfig.direction === null) {
      return <ArrowUpDown className="w-3 h-3 inline ml-1 text-slate-300" />;
    }
    if (sortConfig.direction === 'asc') {
      return <ArrowUp className="w-3 h-3 inline ml-1 text-primary" />;
    }
    return <ArrowDown className="w-3 h-3 inline ml-1 text-primary" />;
  };

  const initialBMData = [
    { id: 'BM-001-VN', name: 'Wemake Official VN', count: 12, status: 'VERIFIED', limit: '$50,000', manager: 'Nguyễn Admin', color: 'teal' },
    { id: 'BM-002-VN', name: 'Wemake Ads Agency', count: 8, status: 'VERIFIED', limit: '$25,000', manager: 'Trần Minh', color: 'teal' },
    { id: 'BM-003-US', name: 'Wemake Global', count: 5, status: 'RESTRICTED', limit: '$10,000', manager: 'John Doe', color: 'rose' },
    { id: 'BM-004-VN', name: 'Wemake Ecom Store', count: 3, status: 'VERIFIED', limit: '$15,000', manager: 'Lê Thị Hoa', color: 'teal' },
    { id: 'BM-005-SG', name: 'Wemake SEA Hub', count: 20, status: 'UNDER REVIEW', limit: '$100,000', manager: 'David Tan', color: 'amber' },
  ];

  const [bmData, setBMData] = useState(initialBMData);
  const [bmSortConfig, setBMSortConfig] = useState<{ key: string; direction: 'asc' | 'desc' | null }>({ key: '', direction: null });
  const [selectedBMRows, setSelectedBMRows] = useState<string[]>([]);
  const [bmStatusFilter, setBMStatusFilter] = useState<string>('ALL');

  const handleBMSort = (key: string) => {
    let direction: 'asc' | 'desc' | null = 'asc';
    if (bmSortConfig.key === key && bmSortConfig.direction === 'asc') {
      direction = 'desc';
    } else if (bmSortConfig.key === key && bmSortConfig.direction === 'desc') {
      direction = null;
    }

    setBMSortConfig({ key, direction });

    if (direction === null) {
      setBMData(initialBMData);
      return;
    }

    const sortedData = [...bmData].sort((a: any, b: any) => {
      let valA = a[key];
      let valB = b[key];

      if (key === 'count') {
        valA = parseInt(valA);
        valB = parseInt(valB);
      }
      if (key === 'limit') {
        valA = parseFloat(valA.replace(/[$,]/g, ''));
        valB = parseFloat(valB.replace(/[$,]/g, ''));
      }

      if (valA < valB) return direction === 'asc' ? -1 : 1;
      if (valA > valB) return direction === 'asc' ? 1 : -1;
      return 0;
    });

    setBMData(sortedData);
  };

  const getBMSortIcon = (key: string) => {
    if (bmSortConfig.key !== key || bmSortConfig.direction === null) {
      return <ArrowUpDown className="w-3 h-3 inline ml-1 text-slate-300" />;
    }
    if (bmSortConfig.direction === 'asc') {
      return <ArrowUp className="w-3 h-3 inline ml-1 text-primary" />;
    }
    return <ArrowDown className="w-3 h-3 inline ml-1 text-primary" />;
  };

  const toggleBMSelectAll = () => {
    if (selectedBMRows.length === bmData.length) {
      setSelectedBMRows([]);
    } else {
      setSelectedBMRows(bmData.map(row => row.id));
    }
  };

  const toggleBMSelectRow = (id: string) => {
    if (selectedBMRows.includes(id)) {
      setSelectedBMRows(selectedBMRows.filter(rowId => rowId !== id));
    } else {
      setSelectedBMRows([...selectedBMRows, id]);
    }
  };

  const filteredBMData = bmData.filter(row => {
    if (bmStatusFilter === 'ALL') return true;
    return row.status === bmStatusFilter;
  });

  const initialLogData = [
    { time: '08:45:12', user: 'admin@wemake.vn', action: 'Đăng nhập', object: 'Dashboard', ip: '192.168.1.101', color: 'sky' },
    { time: '08:47:30', user: 'admin@wemake.vn', action: 'Tạo chiến dịch', object: 'Campaign_Spring_2026', ip: '192.168.1.101', color: 'teal' },
    { time: '09:12:05', user: 'marketing@wemake.vn', action: 'Chỉnh sửa TKQC', object: 'TKQC_Campaign_A', ip: '10.0.0.55', color: 'amber' },
    { time: '09:30:18', user: 'support@wemake.vn', action: 'Xuất báo cáo', object: 'Report_March_2026.xlsx', ip: '10.0.0.88', color: 'slate' },
    { time: '10:05:42', user: 'dev@wemake.vn', action: 'Xóa tài khoản', object: 'TKQC_Test_Old', ip: '172.16.0.12', color: 'rose' },
    { time: '10:15:00', user: 'admin@wemake.vn', action: 'Cập nhật proxy', object: 'Proxy Pool #3', ip: '192.168.1.101', color: 'amber' },
    { time: '11:00:33', user: 'marketing@wemake.vn', action: 'Upload media', object: 'banner_summer.png', ip: '10.0.0.55', color: 'teal' },
  ];

  const [logData, setLogData] = useState(initialLogData);
  const [logSortConfig, setLogSortConfig] = useState<{ key: string; direction: 'asc' | 'desc' | null }>({ key: '', direction: null });
  const [logActionFilter, setLogActionFilter] = useState<string>('ALL');

  const handleLogSort = (key: string) => {
    let direction: 'asc' | 'desc' | null = 'asc';
    if (logSortConfig.key === key && logSortConfig.direction === 'asc') {
      direction = 'desc';
    } else if (logSortConfig.key === key && logSortConfig.direction === 'desc') {
      direction = null;
    }

    setLogSortConfig({ key, direction });

    if (direction === null) {
      setLogData(initialLogData);
      return;
    }

    const sortedData = [...logData].sort((a: any, b: any) => {
      let valA = a[key];
      let valB = b[key];

      if (valA < valB) return direction === 'asc' ? -1 : 1;
      if (valA > valB) return direction === 'asc' ? 1 : -1;
      return 0;
    });

    setLogData(sortedData);
  };

  const getLogSortIcon = (key: string) => {
    if (logSortConfig.key !== key || logSortConfig.direction === null) {
      return <ArrowUpDown className="w-3 h-3 inline ml-1 text-slate-300" />;
    }
    if (logSortConfig.direction === 'asc') {
      return <ArrowUp className="w-3 h-3 inline ml-1 text-primary" />;
    }
    return <ArrowDown className="w-3 h-3 inline ml-1 text-primary" />;
  };

  const filteredLogData = logData.filter(row => {
    if (logActionFilter === 'ALL') return true;
    return row.action === logActionFilter;
  });

  const initialMemberData = [
    { name: 'Nguyễn Văn Admin', email: 'admin@wemake.vn', role: 'Admin', last: '23/03/2026 08:30', online: true },
    { name: 'Trần Thị Marketing', email: 'marketing@wemake.vn', role: 'Editor', last: '23/03/2026 09:15', online: true },
    { name: 'Lê Minh Dev', email: 'dev@wemake.vn', role: 'Developer', last: '22/03/2026 18:00', online: false },
    { name: 'Phạm Hồng Support', email: 'support@wemake.vn', role: 'Viewer', last: '23/03/2026 07:45', online: true },
    { name: 'Võ Đức Manager', email: 'manager@wemake.vn', role: 'Admin', last: '21/03/2026 16:20', online: false },
  ];

  const [memberData, setMemberData] = useState(initialMemberData);
  const [memberSortConfig, setMemberSortConfig] = useState<{ key: string; direction: 'asc' | 'desc' | null }>({ key: '', direction: null });
  const [memberRoleFilter, setMemberRoleFilter] = useState<string>('ALL');
  const [demoState, setDemoState] = useState<'loading' | 'empty' | 'loaded'>('loading');
  const [memberStatusFilter, setMemberStatusFilter] = useState<string>('ALL');
  const [isFinished, setIsFinished] = useState(false);

  const handleMemberSort = (key: string) => {
    let direction: 'asc' | 'desc' | null = 'asc';
    if (memberSortConfig.key === key && memberSortConfig.direction === 'asc') {
      direction = 'desc';
    } else if (memberSortConfig.key === key && memberSortConfig.direction === 'desc') {
      direction = null;
    }

    setMemberSortConfig({ key, direction });

    if (direction === null) {
      setMemberData(initialMemberData);
      return;
    }

    const sortedData = [...memberData].sort((a: any, b: any) => {
      let valA = a[key];
      let valB = b[key];

      if (key === 'last') {
        const [dateA, timeA] = valA.split(' ');
        const [dayA, monthA, yearA] = dateA.split('/');
        const [hourA, minA] = timeA.split(':');
        valA = new Date(`${yearA}-${monthA}-${dayA}T${hourA}:${minA}:00`).getTime();

        const [dateB, timeB] = valB.split(' ');
        const [dayB, monthB, yearB] = dateB.split('/');
        const [hourB, minB] = timeB.split(':');
        valB = new Date(`${yearB}-${monthB}-${dayB}T${hourB}:${minB}:00`).getTime();
      }

      if (key === 'online') {
        valA = valA ? 1 : 0;
        valB = valB ? 1 : 0;
      }

      if (valA < valB) return direction === 'asc' ? -1 : 1;
      if (valA > valB) return direction === 'asc' ? 1 : -1;
      return 0;
    });

    setMemberData(sortedData);
  };

  const getMemberSortIcon = (key: string) => {
    if (memberSortConfig.key !== key || memberSortConfig.direction === null) {
      return <ArrowUpDown className="w-3 h-3 inline ml-1 text-slate-300" />;
    }
    if (memberSortConfig.direction === 'asc') {
      return <ArrowUp className="w-3 h-3 inline ml-1 text-primary" />;
    }
    return <ArrowDown className="w-3 h-3 inline ml-1 text-primary" />;
  };

  const filteredMemberData = memberData.filter(row => {
    const roleMatch = memberRoleFilter === 'ALL' || row.role === memberRoleFilter;
    const statusMatch = memberStatusFilter === 'ALL' || (memberStatusFilter === 'Online' ? row.online : !row.online);
    return roleMatch && statusMatch;
  });

  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const totalItems = 156;

  const totalPages = Math.ceil(totalItems / pageSize);
  const startItem = (currentPage - 1) * pageSize + 1;
  const endItem = Math.min(currentPage * pageSize, totalItems);

  const [modalConfig, setModalConfig] = useState<{
    isOpen: boolean;
    type: 'info' | 'success' | 'error' | 'question' | 'default' | 'delete' | 'warning' | 'sidebar' | 'result' | 'config' | 'campaign-form' | 'system-info';
    title: string;
    message: string;
  } | null>(null);

  // Campaign Form State
  const [campaignName, setCampaignName] = useState('');
  const [campaignMethod, setCampaignMethod] = useState('');
  const [campaignStartDate, setCampaignStartDate] = useState('');
  const [campaignBudget, setCampaignBudget] = useState(40);
  const [campaignNotes, setCampaignNotes] = useState('');

  const [activeConfigTab, setActiveConfigTab] = useState<'proxy' | 'chung' | 'nang-cao'>('proxy');
  const [proxyUrl, setProxyUrl] = useState('');
  const [proxyUsername, setProxyUsername] = useState('');
  const [proxyPassword, setProxyPassword] = useState('');
  const [showPassword, setShowPassword] = useState(false);
  const [autoRetry, setAutoRetry] = useState(true);
  const [detailedLog, setDetailedLog] = useState(false);
  const [selectedMode, setSelectedMode] = useState<1 | 2>(1);

  const [isRechecking, setIsRechecking] = useState(false);
  const [activeTokenSection, setActiveTokenSection] = useState('brand-colors');

  const handleRecheck = () => {
    setIsRechecking(true);
    setTimeout(() => {
      setIsRechecking(false);
    }, 1500);
  };

  const [toasts, setToasts] = useState<{ id: number; type: 'info' | 'success' | 'warning' | 'error'; title: string; message: string }[]>([]);

  const addToast = (type: 'info' | 'success' | 'warning' | 'error', title: string, message: string) => {
    const id = Date.now();
    setToasts(prev => [...prev, { id, type, title, message }]);
    setTimeout(() => {
      setToasts(prev => prev.filter(t => t.id !== id));
    }, 5000);
  };

  const removeToast = (id: number) => {
    setToasts(prev => prev.filter(t => t.id !== id));
  };

  const toggleNode = (nodeId: string) => {
    setExpandedNodes(prev => 
      prev.includes(nodeId) ? prev.filter(id => id !== nodeId) : [...prev, nodeId]
    );
  };

  // Esc key listener for drawer
  useEffect(() => {
    const handleEsc = (e: KeyboardEvent) => {
      if (e.key === 'Escape') setIsDrawerOpen(false);
    };
    window.addEventListener('keydown', handleEsc);
    return () => window.removeEventListener('keydown', handleEsc);
  }, []);

  const copyToClipboard = (text: string) => {
    navigator.clipboard.writeText(text);
    addToast('success', 'Đã sao chép', `Mã màu ${text} đã được sao chép`);
  };

  return (
    <div className={`min-h-screen flex flex-col transition-colors duration-300 ${darkMode ? 'dark bg-surface text-on-surface' : 'bg-surface text-on-surface'}`}>
      {/* HEADER */}
      <header className="sticky top-0 z-50 flex flex-col shadow-sm">
        {/* Top Bar */}
        <div className="header-gradient text-white">
          <div className="max-w-6xl mx-auto w-full px-8 py-6 flex items-center justify-between">
            <div className="flex items-center space-x-4">
              <div className="flex items-center space-x-2">
                <div className="w-9 h-9 bg-white/20 rounded flex items-center justify-center">
                  <Bolt className="w-5.5 h-5.5 text-white" />
                </div>
                <div>
                  <h1 className="text-lg font-bold leading-tight">WEMAKE</h1>
                  <p className="text-[10px] text-white/70 font-medium uppercase tracking-wider">Wemake System UI</p>
                </div>
              </div>
              <div className="h-8 w-px bg-white/20 mx-2" />
              <p className="text-xs font-medium text-white/90 hidden md:block">
                Hệ thống UI Components dành cho các sản phẩm Wemake
              </p>
            </div>
            
            <div className="flex items-center space-x-4">
              <div className="hidden sm:flex items-center space-x-2 bg-white/10 px-3 py-1.5 rounded-full border border-white/10">
                <CheckCircle2 className="w-3.5 h-3.5 text-teal-400" />
                <span className="text-[11px] font-semibold">Build: <span className="text-white">OK</span></span>
                <span className="text-white/30">|</span>
                <span className="text-[11px]">91 files</span>
              </div>
              
              <button 
                onClick={() => setDarkMode(!darkMode)}
                className="flex items-center space-x-2 px-3 py-2 bg-slate-950 hover:bg-black text-white rounded-full border border-white/10 transition-all shadow-lg active:scale-95 group"
              >
                {darkMode ? <Sun className="w-4 h-4 text-yellow-400" /> : <Moon className="w-4 h-4 text-slate-300" />}
                <span className="text-[10px] font-bold uppercase tracking-wider hidden xs:block">
                  {darkMode ? 'Light' : 'Dark'}
                </span>
              </button>
            </div>
          </div>
        </div>

        {/* Navigation Bar */}
        <nav className="bg-surface-container-lowest border-b border-outline-variant transition-colors duration-300">
          <div className="max-w-6xl mx-auto w-full px-8 py-4 flex items-center overflow-x-auto no-scrollbar">
            <div className="flex items-center space-x-1">
              <button 
                onClick={() => setActivePage('primitives')}
                className={`flex items-center space-x-2 px-5 py-2 rounded-lg text-sm font-semibold transition-all ${activePage === 'primitives' ? 'btn-primary shadow-lg shadow-primary/30' : 'text-on-surface-variant hover:bg-surface-container'}`}
              >
                <Bolt className="w-4 h-4" />
                <span>Primitives</span>
              </button>
              <button 
                onClick={() => setActivePage('patterns')}
                className={`flex items-center space-x-2 px-5 py-2 rounded-lg text-sm font-semibold transition-all ${activePage === 'patterns' ? 'btn-primary shadow-lg shadow-primary/30' : 'text-on-surface-variant hover:bg-surface-container'}`}
              >
                <div className={`w-4 h-4 border-2 rounded-sm transition-colors ${activePage === 'patterns' ? 'border-white' : 'border-slate-300'}`} />
                <span>Patterns</span>
              </button>
              <button 
                onClick={() => setActivePage('advanced')}
                className={`flex items-center space-x-2 px-5 py-2 rounded-lg text-sm font-semibold transition-all ${activePage === 'advanced' ? 'btn-primary shadow-lg shadow-primary/30' : 'text-on-surface-variant hover:bg-surface-container'}`}
              >
                <Layout className="w-4 h-4" />
                <span>Advanced</span>
              </button>
              <button 
                onClick={() => setActivePage('dialogs')}
                className={`flex items-center space-x-2 px-5 py-2 rounded-lg text-sm font-semibold transition-all ${activePage === 'dialogs' ? 'btn-primary shadow-lg shadow-primary/30' : 'text-on-surface-variant hover:bg-surface-container'}`}
              >
                <MessageSquare className="w-4 h-4" />
                <span>Dialogs</span>
              </button>
              <button 
                onClick={() => setActivePage('tokens')}
                className={`flex items-center space-x-2 px-5 py-2 rounded-lg text-sm font-semibold transition-all ${activePage === 'tokens' ? 'btn-primary shadow-lg shadow-primary/30' : 'text-on-surface-variant hover:bg-surface-container'}`}
              >
                <Coins className="w-4 h-4" />
                <span>Tokens</span>
              </button>
            </div>
          </div>
        </nav>
      </header>

      {/* MAIN CONTENT */}
      <main className="flex-1 max-w-6xl mx-auto w-full p-8 space-y-12 transition-colors duration-300">
        {activePage === 'primitives' && (
          <>
            {/* WxButton Section */}
            <section className="space-y-6">
          <div className="flex items-center space-x-2 text-primary font-bold text-lg">
            <Bolt className="w-5 h-5" />
            <span>WxButton</span>
          </div>
          <div className="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant space-y-8 transition-colors duration-300">
            <div>
              <h3 className="text-[10px] font-bold text-on-surface-variant opacity-60 uppercase tracking-wider mb-4">Variants</h3>
              <div className="flex flex-wrap gap-4 items-center">
                <button className="px-6 py-2 bg-gradient-to-r from-cyan-400 to-blue-600 text-white font-bold rounded-md text-sm shadow-lg shadow-blue-500/25 hover:-translate-y-0.5 transition-all">Primary</button>
                <button className="px-6 py-2 bg-gradient-to-r from-cyan-400 to-blue-700 text-white font-bold rounded-md text-sm shadow-xl shadow-blue-600/30 hover:-translate-y-0.5 transition-all">CTA</button>
                <button className="px-6 py-2 bg-white border border-slate-100 text-slate-600 font-bold rounded-md text-sm shadow-sm hover:bg-slate-50 transition-all">Secondary</button>
                <button className="px-6 py-2 text-slate-500 hover:text-slate-700 rounded-md text-sm font-bold transition-all">Ghost</button>
                <button className="px-6 py-2 bg-[#F43F5E] text-white font-bold rounded-md text-sm shadow-lg shadow-rose-500/25 hover:-translate-y-0.5 transition-all">Danger</button>
                <button className="px-6 py-2 bg-[#10B981] text-white font-bold rounded-md text-sm shadow-lg shadow-emerald-500/25 hover:-translate-y-0.5 transition-all">Success</button>
                <button className="px-6 py-2 text-slate-500 text-sm font-bold hover:text-primary transition-all">Text</button>
              </div>
            </div>
            
            <div>
              <h3 className="text-[10px] font-bold text-on-surface-variant opacity-60 uppercase tracking-wider mb-4">Sizes</h3>
              <div className="flex flex-wrap gap-4 items-end">
                <button className="px-3 py-1 bg-gradient-to-r from-cyan-400 to-blue-600 text-white font-bold rounded-md text-[10px] shadow-md shadow-blue-500/20 hover:-translate-y-0.5 transition-all">Small</button>
                <button className="px-6 py-2 bg-gradient-to-r from-cyan-400 to-blue-600 text-white font-bold rounded-md text-sm shadow-lg shadow-blue-500/25 hover:-translate-y-0.5 transition-all">Medium</button>
                <button className="px-10 py-3 bg-gradient-to-r from-cyan-400 to-blue-600 text-white font-bold rounded-md text-lg shadow-xl shadow-blue-500/30 hover:-translate-y-0.5 transition-all">Large</button>
              </div>
            </div>

            <div>
              <h3 className="text-[10px] font-bold text-on-surface-variant opacity-60 uppercase tracking-wider mb-4">States</h3>
              <div className="flex flex-wrap gap-4 items-center">
                <button className="flex items-center space-x-2 px-6 py-2 bg-gradient-to-r from-cyan-400 to-blue-600 text-white font-bold rounded-md text-sm shadow-lg shadow-blue-500/25 hover:-translate-y-0.5 transition-all">
                  <Check className="w-4 h-4" />
                  <span>With Icon</span>
                </button>
                <button className="flex items-center space-x-2 px-6 py-2 bg-sky-400 text-white font-bold rounded-md text-sm shadow-md shadow-sky-400/20 cursor-wait">
                  <Loader2 className="w-4 h-4 animate-spin" />
                  <span>Loading...</span>
                </button>
                <button className="px-6 py-2 bg-sky-200 text-white/80 font-bold rounded-md text-sm cursor-not-allowed" disabled>Disabled</button>
                <button className="flex items-center space-x-2 px-6 py-2 bg-[#F43F5E] text-white font-bold rounded-md text-sm shadow-lg shadow-rose-500/25 hover:-translate-y-0.5 transition-all">
                  <Trash2 className="w-4 h-4" />
                  <span>Delete</span>
                </button>
                <button className="p-2 text-slate-500 hover:text-primary transition-all hover:scale-110 active:scale-95"><Settings className="w-5 h-5" /></button>
                <button className="p-2 text-slate-500 hover:text-primary transition-all hover:scale-110 active:scale-95"><Bell className="w-5 h-5" /></button>
              </div>
            </div>
          </div>
        </section>

        {/* Inputs Section */}
        <section className="space-y-6">
          <div className="flex items-center space-x-2 text-primary font-bold text-lg">
            <Bolt className="w-5 h-5" />
            <span>Inputs</span>
          </div>
          <div className="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant transition-colors duration-300">
            <div className="grid grid-cols-1 md:grid-cols-3 gap-x-12 gap-y-8">
              <div className="space-y-2">
                <label className="text-[10px] font-bold text-on-surface-variant opacity-60 uppercase tracking-wider">WXINPUT</label>
                <div className="relative">
                  <User className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-on-surface-variant opacity-60" />
                  <input 
                    type="text" 
                    placeholder="Nhập text..." 
                    className="w-full pl-10 pr-4 py-2 bg-surface-container-low border border-outline-variant rounded-lg text-sm text-on-surface placeholder:text-on-surface-variant placeholder:opacity-40 focus:outline-none focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all"
                  />
                </div>
                <p className="text-[10px] text-on-surface-variant opacity-60 italic">Hỗ trợ icon, clearable, error</p>
              </div>

              <div className="space-y-2">
                <label className="text-[10px] font-bold text-on-surface-variant opacity-60 uppercase tracking-wider invisible">Search</label>
                <div className="relative">
                  <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-on-surface-variant opacity-60" />
                  <input 
                    type="text" 
                    placeholder="Tìm kiếm..." 
                    className="w-full pl-10 pr-4 py-2 bg-surface-container-low border border-outline-variant rounded-lg text-sm text-on-surface placeholder:text-on-surface-variant placeholder:opacity-40 focus:outline-none focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all"
                  />
                </div>
              </div>

              <div className="space-y-2">
                <label className="text-[10px] font-bold text-on-surface-variant opacity-60 uppercase tracking-wider">WXPASSWORDINPUT</label>
                <div className="relative">
                  <Lock className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-on-surface-variant opacity-60" />
                  <input 
                    type={isPasswordVisible ? "text" : "password"} 
                    placeholder="Nhập mật khẩu..." 
                    className="w-full pl-10 pr-10 py-2 bg-surface-container-low border border-outline-variant rounded-lg text-sm text-on-surface placeholder:text-on-surface-variant placeholder:opacity-40 focus:outline-none focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all"
                  />
                  <button 
                    onClick={() => setIsPasswordVisible(!isPasswordVisible)}
                    className="absolute right-3 top-1/2 -translate-y-1/2 text-on-surface-variant opacity-60 hover:text-on-surface"
                  >
                    <Eye className="w-4 h-4" />
                  </button>
                </div>
              </div>

              <div className="space-y-2">
                <label className="text-[10px] font-bold text-on-surface-variant opacity-60 uppercase tracking-wider">WXTEXTAREA</label>
                <textarea 
                  placeholder="Nhập nội dung dài..." 
                  className="w-full px-4 py-2 bg-surface-container-low border border-outline-variant rounded-lg text-sm text-on-surface placeholder:text-on-surface-variant placeholder:opacity-40 focus:outline-none focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all h-24 resize-none"
                />
              </div>

              <div className="space-y-2">
                <label className="text-[10px] font-bold text-on-surface-variant opacity-60 uppercase tracking-wider">WXINPUTNUMBER</label>
                <input 
                  type="number" 
                  defaultValue={5}
                  className="w-full px-4 py-2 bg-surface-container-low border border-outline-variant rounded-lg text-sm text-on-surface focus:outline-none focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all"
                />
              </div>

              <div className="space-y-2">
                <label className="text-[10px] font-bold text-on-surface-variant opacity-60 uppercase tracking-wider">ERROR STATE</label>
                <input 
                  type="text" 
                  placeholder="Required..." 
                  className="w-full px-4 py-2 bg-rose-500/10 border border-rose-500/30 rounded-lg text-sm text-on-surface placeholder:text-on-surface-variant placeholder:opacity-40 focus:outline-none focus:ring-2 focus:ring-rose-500/20 focus:border-rose-500 transition-all"
                />
                <p className="text-[10px] text-rose-500 font-medium">Trường này bắt buộc</p>
              </div>
            </div>
          </div>
        </section>

        {/* Select & MultiSelect Section */}
        <section className="space-y-6">
          <div className="flex items-center space-x-2 text-primary font-bold text-lg">
            <Bolt className="w-5 h-5" />
            <span>Select & MultiSelect</span>
          </div>
          <div className="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant transition-colors duration-300">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-12">
              <div className="space-y-2">
                <label className="text-[10px] font-bold text-on-surface-variant opacity-60 uppercase tracking-wider">WXSELECT</label>
                <div className="relative">
                  <select className="w-full appearance-none px-4 py-2 bg-surface-container-low border border-outline-variant rounded-lg text-sm text-on-surface-variant opacity-80 focus:outline-none focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all">
                    <option>Chọn phương thức...</option>
                    <option>Chuyển khoản ngân hàng</option>
                    <option>Ví điện tử Momo</option>
                    <option>Thẻ tín dụng</option>
                  </select>
                  <ChevronDown className="absolute right-3 top-1/2 -translate-y-1/2 w-4 h-4 text-on-surface-variant opacity-60 pointer-events-none" />
                </div>
              </div>
              <div className="space-y-2">
                <label className="text-[10px] font-bold text-on-surface-variant opacity-60 uppercase tracking-wider">WXMULTISELECT</label>
                <div className="relative">
                  <select className="w-full appearance-none px-4 py-2 bg-surface-container-low border border-outline-variant rounded-lg text-sm text-on-surface-variant opacity-80 focus:outline-none focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all">
                    <option>Chọn nhiều...</option>
                  </select>
                  <ChevronDown className="absolute right-3 top-1/2 -translate-y-1/2 w-4 h-4 text-on-surface-variant opacity-60 pointer-events-none" />
                </div>
              </div>
            </div>
          </div>
        </section>

        {/* WxDropdown Section */}
        <section className="space-y-6">
          <div className="flex items-center space-x-2 text-primary font-bold text-lg">
            <Bolt className="w-5 h-5" />
            <span>WxDropdown</span>
          </div>
          <div className="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant flex flex-wrap gap-8 transition-colors duration-300">
            <div className="relative group">
              <button className="px-6 py-2 btn-secondary rounded-md text-sm flex items-center space-x-2">
                <span>Thao tác</span>
                <ChevronDown className="w-4 h-4" />
              </button>
              <div className="absolute top-full left-0 mt-2 w-48 bg-surface-container-lowest border border-outline-variant rounded-xl shadow-xl opacity-0 invisible group-hover:opacity-100 group-hover:visible transition-all z-10 p-1.5">
                <button 
                  className="w-full text-left px-3 py-2 text-sm text-on-surface-variant hover:bg-surface-container rounded-lg flex items-center space-x-2 transition-colors"
                >
                  <FileText className="w-4 h-4" />
                  <span>Xem chi tiết</span>
                </button>
                <button 
                  className="w-full text-left px-3 py-2 text-sm text-on-surface-variant hover:bg-surface-container rounded-lg flex items-center space-x-2 transition-colors"
                >
                  <Settings className="w-4 h-4" />
                  <span>Chỉnh sửa</span>
                </button>
                <div className="h-px bg-outline-variant my-1 mx-2" />
                <button 
                  className="w-full text-left px-3 py-2 text-sm text-rose-500 hover:bg-rose-500/10 rounded-lg flex items-center space-x-2 transition-colors"
                >
                  <Trash2 className="w-4 h-4" />
                  <span>Xóa dữ liệu</span>
                </button>
              </div>
            </div>

            <div className="relative group">
              <button className="p-2 btn-secondary rounded-lg text-on-surface-variant opacity-60 hover:text-primary">
                <Settings className="w-5 h-5" />
              </button>
              <div className="absolute top-full right-0 mt-2 w-56 bg-surface-container-lowest border border-outline-variant rounded-xl shadow-xl opacity-0 invisible group-hover:opacity-100 group-hover:visible transition-all z-10 p-4 space-y-4">
                <div className="flex items-center space-x-3">
                  <div className="w-10 h-10 rounded-full bg-primary text-white flex items-center justify-center font-bold">AU</div>
                  <div>
                    <p className="text-sm font-bold text-on-surface">Admin User</p>
                    <p className="text-[10px] text-on-surface-variant opacity-60">admin@wemake.vn</p>
                  </div>
                </div>
                <div className="h-px bg-outline-variant" />
                <div className="space-y-1">
                  <button 
                    className="w-full text-left px-2 py-1.5 text-xs font-semibold text-on-surface-variant opacity-60 hover:text-primary transition-colors"
                  >
                    Hồ sơ cá nhân
                  </button>
                  <button 
                    className="w-full text-left px-2 py-1.5 text-xs font-semibold text-on-surface-variant opacity-60 hover:text-primary transition-colors"
                  >
                    Cài đặt hệ thống
                  </button>
                  <button 
                    className="w-full text-left px-2 py-1.5 text-xs font-semibold text-rose-500 hover:text-rose-600 transition-colors"
                  >
                    Đăng xuất
                  </button>
                </div>
              </div>
            </div>
          </div>
        </section>

        {/* Controls Section */}
        <section className="space-y-6">
          <div className="flex items-center space-x-2 text-primary font-bold text-lg">
            <Bolt className="w-5 h-5" />
            <span>Controls</span>
          </div>
          <div className="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant flex flex-wrap gap-x-24 gap-y-8 transition-colors duration-300">
            <div className="space-y-4">
              <h3 className="text-[10px] font-bold text-on-surface-variant opacity-60 uppercase tracking-wider">WXTOGGLE</h3>
              <div className="space-y-3">
                <div className="flex items-center space-x-3">
                  <button 
                    onClick={() => setToggleOn(!toggleOn)}
                    className={`w-10 h-5 rounded-full relative transition-all duration-300 ${toggleOn ? 'btn-primary shadow-md shadow-primary/20' : 'bg-surface-container'}`}
                  >
                    <motion.div 
                      animate={{ x: toggleOn ? 20 : 2 }}
                      className="absolute top-1 w-3 h-3 bg-white rounded-full shadow-sm"
                    />
                  </button>
                  <span className="text-xs text-on-surface-variant opacity-80 font-medium">Bật/Tắt</span>
                </div>
                <div className="flex items-center space-x-3">
                  <div className="w-10 h-5 bg-surface-container rounded-full relative">
                    <div className="absolute top-1 left-1 w-3 h-3 bg-surface-container-lowest rounded-full shadow-sm" />
                  </div>
                  <span className="text-xs text-on-surface-variant opacity-40 font-medium">Chế độ khác</span>
                </div>
              </div>
            </div>

            <div className="space-y-4">
              <h3 className="text-[10px] font-bold text-on-surface-variant opacity-60 uppercase tracking-wider">WXRADIO</h3>
              <div className="space-y-3">
                {['A', 'B', 'C'].map((val) => (
                  <label key={val} className="flex items-center space-x-3 cursor-pointer group">
                    <div className="relative flex items-center justify-center">
                      <input 
                        type="radio" 
                        name="wxradio" 
                        className="sr-only" 
                        checked={radioValue === val}
                        onChange={() => setRadioValue(val)}
                      />
                      <div className={`w-4 h-4 rounded-full border transition-all ${radioValue === val ? 'border-primary' : 'border-outline group-hover:border-outline-variant'}`} />
                      {radioValue === val && (
                        <motion.div layoutId="radio-dot" className="absolute w-2 h-2 bg-primary rounded-full" />
                      )}
                    </div>
                    <span className={`text-xs font-medium transition-colors ${radioValue === val ? 'text-on-surface' : 'text-on-surface-variant opacity-60'}`}>Option {val}</span>
                  </label>
                ))}
              </div>
            </div>

            <div className="space-y-4">
              <h3 className="text-[10px] font-bold text-on-surface-variant opacity-60 uppercase tracking-wider">WXCHECKBOX</h3>
              <label className="flex items-center space-x-3 cursor-pointer group">
                <div className="w-4 h-4 rounded border border-outline group-hover:border-outline-variant flex items-center justify-center transition-all bg-surface-container-lowest overflow-hidden">
                  <input 
                    type="checkbox" 
                    className="sr-only peer" 
                    checked={checkboxChecked}
                    onChange={(e) => setCheckboxChecked(e.target.checked)}
                  />
                  <div className="w-full h-full bg-primary opacity-0 peer-checked:opacity-100 flex items-center justify-center transition-opacity">
                    <Check className="w-3 h-3 text-white" strokeWidth={3} />
                  </div>
                </div>
                <span className="text-xs text-on-surface-variant opacity-80 font-medium">Đồng ý điều khoản</span>
              </label>
            </div>
          </div>
        </section>

        {/* WxTabs Section */}
        <section className="space-y-6">
          <div className="flex items-center space-x-2 text-primary font-bold text-lg">
            <Bolt className="w-5 h-5" />
            <span>WxTabs</span>
          </div>
          <div className="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant space-y-10 transition-colors duration-300">
            <div className="space-y-4">
              <h3 className="text-[10px] font-bold text-on-surface-variant opacity-60 uppercase tracking-wider">UNDERLINE VARIANT</h3>
              <div className="flex border-b border-outline-variant">
                <button 
                  onClick={() => setActiveTab('general')}
                  className={`px-4 py-2 flex items-center space-x-2 text-sm transition-all relative ${activeTab === 'general' ? 'tab-active' : 'text-on-surface-variant opacity-60 hover:text-on-surface'}`}
                >
                  <Home className="w-4 h-4" />
                  <span>Tổng quan</span>
                  {activeTab === 'general' && (
                    <motion.div layoutId="tab-underline" className="absolute bottom-0 left-0 right-0 h-0.5 bg-primary rounded-full shadow-[0_-2px_8px_rgba(0,136,255,0.4)]" />
                  )}
                </button>
                <button 
                  onClick={() => setActiveTab('settings')}
                  className={`px-4 py-2 flex items-center space-x-2 text-sm transition-all relative ${activeTab === 'settings' ? 'tab-active' : 'text-on-surface-variant opacity-60 hover:text-on-surface'}`}
                >
                  <Settings className="w-4 h-4" />
                  <span>Cài đặt</span>
                  <span className={`text-[10px] px-1.5 py-0.5 rounded-full font-bold ${activeTab === 'settings' ? 'bg-primary/10 text-primary' : 'bg-surface-container text-on-surface-variant opacity-60'}`}>3</span>
                  {activeTab === 'settings' && (
                    <motion.div layoutId="tab-underline" className="absolute bottom-0 left-0 right-0 h-0.5 bg-primary rounded-full shadow-[0_-2px_8px_rgba(0,136,255,0.4)]" />
                  )}
                </button>
                <button 
                  onClick={() => setActiveTab('logs')}
                  className={`px-4 py-2 flex items-center space-x-2 text-sm transition-all relative ${activeTab === 'logs' ? 'tab-active' : 'text-on-surface-variant opacity-60 hover:text-on-surface'}`}
                >
                  <List className="w-4 h-4" />
                  <span>Nhật ký</span>
                  {activeTab === 'logs' && (
                    <motion.div layoutId="tab-underline" className="absolute bottom-0 left-0 right-0 h-0.5 bg-primary rounded-full shadow-[0_-2px_8px_rgba(0,136,255,0.4)]" />
                  )}
                </button>
              </div>
              <div className="p-4 bg-surface-container rounded-lg">
                <p className="text-xs text-on-surface-variant opacity-80">Nội dung tab: <strong className="text-on-surface">{activeTab}</strong></p>
              </div>
            </div>

            <div className="space-y-4">
              <h3 className="text-[10px] font-bold text-on-surface-variant opacity-60 uppercase tracking-wider">PILLS VARIANT</h3>
              <div className="flex space-x-2">
                <button 
                  onClick={() => setActiveTab('general')}
                  className={`px-5 py-2 rounded-lg text-sm font-semibold flex items-center space-x-2 transition-all ${activeTab === 'general' ? 'btn-primary shadow-lg shadow-primary/30' : 'text-on-surface-variant opacity-80 hover:bg-surface-container hover:text-on-surface'}`}
                >
                  <Home className="w-4 h-4" />
                  <span>Tổng quan</span>
                </button>
                <button 
                  onClick={() => setActiveTab('settings')}
                  className={`px-5 py-2 rounded-lg text-sm font-semibold flex items-center space-x-2 transition-all ${activeTab === 'settings' ? 'btn-primary shadow-lg shadow-primary/30' : 'text-on-surface-variant opacity-80 hover:bg-surface-container hover:text-on-surface'}`}
                >
                  <Settings className="w-4 h-4" />
                  <span>Cài đặt</span>
                  <span className={`text-[10px] px-1.5 py-0.5 rounded-full font-bold ${activeTab === 'settings' ? 'bg-white/20 text-white' : 'bg-blue-100 text-primary'}`}>3</span>
                </button>
                <button 
                  onClick={() => setActiveTab('logs')}
                  className={`px-5 py-2 rounded-lg text-sm font-semibold flex items-center space-x-2 transition-all ${activeTab === 'logs' ? 'btn-primary shadow-lg shadow-primary/30' : 'text-on-surface-variant opacity-80 hover:bg-surface-container hover:text-on-surface'}`}
                >
                  <List className="w-4 h-4" />
                  <span>Nhật ký</span>
                </button>
              </div>
            </div>
          </div>
        </section>

        {/* WxAvatar Section */}
        <section className="space-y-6">
          <div className="flex items-center space-x-2 text-primary font-bold text-lg">
            <Bolt className="w-5 h-5" />
            <span>WxAvatar</span>
          </div>
          <div className="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant space-y-10 transition-colors duration-300">
            <div>
              <h3 className="text-[10px] font-bold text-on-surface-variant opacity-60 uppercase tracking-wider mb-6">SIZES</h3>
              <div className="flex items-end gap-6">
                <div className="w-6 h-6 rounded-full bg-amber-400 text-white flex items-center justify-center text-[10px] font-bold">X</div>
                <div className="w-8 h-8 rounded-full bg-rose-500 text-white flex items-center justify-center text-xs font-bold">S</div>
                <div className="w-10 h-10 rounded-full bg-teal-500 text-white flex items-center justify-center text-xs font-bold">NV</div>
                <div className="w-14 h-14 rounded-full bg-amber-500 text-white flex items-center justify-center text-sm font-bold">TB</div>
                <div className="w-20 h-20 rounded-full bg-primary text-white flex items-center justify-center text-xl font-bold">AU</div>
              </div>
            </div>

            <div>
              <h3 className="text-[10px] font-bold text-on-surface-variant opacity-60 uppercase tracking-wider mb-6">WITH IMAGE (LOGO)</h3>
              <div className="flex items-center gap-6">
                <div className="w-10 h-10 rounded-lg bg-surface-container-lowest border border-outline-variant p-1.5 shadow-sm flex items-center justify-center transition-colors duration-300">
                  <Bolt className="w-full h-full text-primary" />
                </div>
                <div className="relative">
                  <div className="w-12 h-12 rounded-lg bg-surface-container-lowest border border-outline-variant p-2 shadow-sm flex items-center justify-center transition-colors duration-300">
                    <Bolt className="w-full h-full text-primary" />
                  </div>
                  <div className="absolute -bottom-1 -right-1 w-4 h-4 bg-teal-500 border-2 border-surface-container-lowest rounded-full transition-colors duration-300" />
                </div>
                <div className="relative">
                  <div className="w-16 h-16 rounded-lg bg-surface-container-lowest border border-outline-variant p-3 shadow-sm flex items-center justify-center transition-colors duration-300">
                    <Bolt className="w-full h-full text-primary" />
                  </div>
                  <div className="absolute -bottom-1 -right-1 w-5 h-5 bg-teal-500 border-2 border-surface-container-lowest rounded-full transition-colors duration-300" />
                </div>
                <div className="w-20 h-20 rounded-lg bg-surface-container-lowest border border-outline-variant p-4 shadow-sm flex items-center justify-center transition-colors duration-300">
                  <Bolt className="w-full h-full text-primary" />
                </div>
              </div>
            </div>

            <div>
              <h3 className="text-[10px] font-bold text-on-surface-variant opacity-60 uppercase tracking-wider mb-6">WITH STATUS</h3>
              <div className="flex gap-6">
                <div className="relative">
                  <div className="w-10 h-10 rounded-full bg-rose-500 text-white flex items-center justify-center text-[10px] font-bold">OU</div>
                  <div className="absolute bottom-0 right-0 w-3 h-3 bg-teal-400 border-2 border-surface-container-lowest rounded-full transition-colors duration-300" />
                </div>
                <div className="relative">
                  <div className="w-10 h-10 rounded-full bg-teal-500 text-white flex items-center justify-center text-[10px] font-bold">BP</div>
                  <div className="absolute bottom-0 right-0 w-3 h-3 bg-teal-400 border-2 border-surface-container-lowest rounded-full transition-colors duration-300" />
                </div>
                <div className="relative">
                  <div className="w-10 h-10 rounded-full bg-fuchsia-500 text-white flex items-center justify-center text-[10px] font-bold">AN</div>
                  <div className="absolute bottom-0 right-0 w-3 h-3 bg-rose-400 border-2 border-surface-container-lowest rounded-full transition-colors duration-300" />
                </div>
                <div className="relative">
                  <div className="w-10 h-10 rounded-full bg-primary text-white flex items-center justify-center text-[10px] font-bold">OA</div>
                  <div className="absolute bottom-0 right-0 w-3 h-3 bg-sky-400 border-2 border-surface-container-lowest rounded-full transition-colors duration-300" />
                </div>
              </div>
            </div>
          </div>
        </section>

        {/* WxTag Section */}
        <section className="space-y-6">
          <div className="flex items-center space-x-2 text-primary font-bold text-lg">
            <Bolt className="w-5 h-5" />
            <span>WxTag</span>
          </div>
          <div className="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant space-y-10 transition-colors duration-300">
            <div>
              <h3 className="text-[10px] font-bold text-on-surface-variant opacity-60 uppercase tracking-wider mb-4">VARIANTS</h3>
              <div className="flex flex-wrap gap-2">
                <span className="px-2.5 py-0.5 bg-surface-container text-on-surface-variant rounded text-[10px] font-bold border border-outline-variant">DEFAULT</span>
                <span className="px-2.5 py-0.5 bg-blue-500/10 text-primary rounded text-[10px] font-bold border border-primary/20">PRIMARY</span>
                <span className="px-2.5 py-0.5 bg-emerald-500/10 text-emerald-500 rounded text-[10px] font-bold border border-emerald-500/20 flex items-center space-x-1">
                  <Check className="w-3 h-3" />
                  <span>SUCCESS</span>
                </span>
                <span className="px-2.5 py-0.5 bg-rose-500/10 text-rose-500 rounded text-[10px] font-bold border border-rose-500/20">DANGER</span>
                <span className="px-2.5 py-0.5 bg-amber-500/10 text-amber-500 rounded text-[10px] font-bold border border-amber-500/20">WARNING</span>
                <span className="px-2.5 py-0.5 bg-sky-500/10 text-sky-500 rounded text-[10px] font-bold border border-sky-500/20">INFO</span>
                <span className="px-2.5 py-0.5 border border-outline-variant text-on-surface-variant opacity-60 rounded text-[10px] font-bold">OUTLINE</span>
              </div>
            </div>

            <div>
              <h3 className="text-[10px] font-bold text-on-surface-variant opacity-60 uppercase tracking-wider mb-4">REMOVABLE</h3>
              <div className="flex flex-wrap gap-2">
                {removableTags.map((i) => (
                  <span key={i} className="px-2.5 py-1 bg-primary/10 text-primary rounded text-[10px] font-bold flex items-center space-x-2">
                    <Bolt className="w-3 h-3" />
                    <span>WxButton</span>
                    <button 
                      onClick={() => setRemovableTags(prev => prev.filter(tag => tag !== i))}
                      className="hover:bg-primary/20 rounded-full p-0.5 transition-colors"
                    >
                      <X className="w-3 h-3" />
                    </button>
                  </span>
                ))}
                {removableTags.length === 0 && (
                  <button 
                    onClick={() => setRemovableTags([1, 2, 3, 4])}
                    className="text-[10px] text-on-surface-variant opacity-60 hover:text-primary transition-colors font-bold"
                  >
                    Khôi phục tags
                  </button>
                )}
              </div>
            </div>
          </div>
        </section>

        {/* Badges Section */}
        <section className="space-y-6">
          <div className="flex items-center space-x-2 text-primary font-bold text-lg">
            <Bolt className="w-5 h-5" />
            <span>Badges</span>
          </div>
          <div className="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant flex items-center gap-4 transition-colors duration-300">
            <span className="px-2 py-0.5 bg-surface-container text-on-surface-variant opacity-60 rounded-full text-[10px] font-bold uppercase">Default</span>
            <span className="px-2 py-0.5 bg-primary text-white rounded-full text-[10px] font-bold uppercase">Primary</span>
            <span className="px-2 py-0.5 bg-primary/10 text-primary rounded-full text-[10px] font-bold uppercase">5</span>
            <div className="flex items-center space-x-2">
              <div className="w-2.5 h-2.5 bg-teal-400 rounded-full" />
              <div className="w-2.5 h-2.5 bg-sky-400 rounded-full" />
              <div className="w-2.5 h-2.5 bg-amber-400 rounded-full" />
              <div className="w-2.5 h-2.5 bg-rose-400 rounded-full" />
            </div>
          </div>
        </section>

        {/* Spinner & ProgressBar Section */}
        <section className="space-y-6">
          <div className="flex items-center space-x-2 text-primary font-bold text-lg">
            <Bolt className="w-5 h-5" />
            <span>Spinner & ProgressBar</span>
          </div>
          <div className="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant space-y-10 transition-colors duration-300">
            <div className="space-y-4">
              <h3 className="text-[10px] font-bold text-on-surface-variant opacity-60 uppercase tracking-wider">WXSPINNER</h3>
              <div className="flex items-center space-x-8">
                <Loader2 className="w-6 h-6 text-primary animate-spin" />
                <div className="flex items-center space-x-2 text-primary font-semibold text-sm">
                  <Loader2 className="w-4 h-4 animate-spin" />
                  <span>Đang tải...</span>
                </div>
                <Loader2 className="w-10 h-10 text-primary animate-spin" />
              </div>
            </div>

            <div className="max-w-md space-y-8">
              <h3 className="text-[10px] font-bold text-on-surface-variant opacity-60 uppercase tracking-wider">WXPROGRESSBAR</h3>
              <div className="space-y-4">
                <div className="space-y-1.5">
                  <div className="flex justify-between text-[10px] font-bold text-on-surface-variant opacity-60 uppercase">
                    <span>Upload</span>
                    <span>65%</span>
                  </div>
                  <div className="h-2 bg-surface-container rounded-full overflow-hidden">
                    <motion.div initial={{ width: 0 }} animate={{ width: '65%' }} className="h-full bg-primary" />
                  </div>
                  <div className="h-2 bg-surface-container rounded-full overflow-hidden">
                    <motion.div initial={{ width: 0 }} animate={{ width: '65%' }} className="h-full bg-emerald-500" />
                  </div>
                </div>

                <div className="space-y-1.5">
                  <div className="flex justify-between text-[10px] font-bold text-on-surface-variant opacity-60 uppercase">
                    <span>Lỗi</span>
                    <span>30%</span>
                  </div>
                  <div className="h-2 bg-surface-container rounded-full overflow-hidden">
                    <motion.div initial={{ width: 0 }} animate={{ width: '30%' }} className="h-full bg-red-500" />
                  </div>
                </div>

                <div className="space-y-1.5">
                  <div className="flex justify-between text-[10px] font-bold text-on-surface-variant opacity-60 uppercase">
                    <span>50%</span>
                  </div>
                  <div className="h-2 bg-surface-container rounded-full overflow-hidden">
                    <motion.div initial={{ width: 0 }} animate={{ width: '50%' }} className="h-full bg-orange-400" />
                  </div>
                  <div className="space-y-1.5">
                    <p className="text-[10px] text-on-surface-variant opacity-40 italic">Đang xử lý...</p>
                    <div className="h-2 bg-surface-container rounded-full overflow-hidden relative">
                      <motion.div 
                        className="absolute h-full bg-sky-400 rounded-full w-1/4"
                        animate={{ 
                          x: ['-100%', '400%'] 
                        }}
                        transition={{ 
                          duration: 2, 
                          repeat: Infinity, 
                          ease: "easeInOut" 
                        }}
                      />
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </section>

        {/* WxPagination Section */}
        <section className="space-y-6">
          <div className="flex items-center space-x-2 text-primary font-bold text-lg">
            <Bolt className="w-5 h-5" />
            <span>WxPagination</span>
          </div>
          <div className="bg-surface-container-lowest p-6 rounded-xl shadow-sm border border-outline-variant flex items-center justify-between transition-colors duration-300">
            <div className="text-xs text-on-surface-variant opacity-60">
              Hiển thị <strong className="text-on-surface">{startItem}-{endItem}</strong> trên <strong className="text-on-surface">{totalItems}</strong>
            </div>
            <div className="flex items-center space-x-1">
              <button 
                disabled={currentPage === 1}
                onClick={() => setCurrentPage(prev => Math.max(1, prev - 1))}
                className={`w-8 h-8 flex items-center justify-center rounded-md transition-colors ${currentPage === 1 ? 'text-on-surface-variant opacity-20 cursor-not-allowed' : 'text-on-surface-variant opacity-60 hover:bg-surface-container'}`}
              >
                <ChevronLeft className="w-4 h-4" />
              </button>
              
              {/* Page Numbers Logic */}
              {(() => {
                const pages = [];
                const maxVisible = 7;
                
                if (totalPages <= maxVisible) {
                  for (let i = 1; i <= totalPages; i++) pages.push(i);
                } else {
                  if (currentPage < 5) {
                    for (let i = 1; i <= 5; i++) pages.push(i);
                    pages.push('...');
                    pages.push(totalPages);
                  } else if (currentPage > totalPages - 4) {
                    pages.push(1);
                    pages.push('...');
                    for (let i = totalPages - 4; i <= totalPages; i++) pages.push(i);
                  } else {
                    pages.push(1);
                    pages.push('...');
                    pages.push(currentPage - 1);
                    pages.push(currentPage);
                    pages.push(currentPage + 1);
                    pages.push('...');
                    pages.push(totalPages);
                  }
                }

                return pages.map((p, i) => {
                  if (p === '...') {
                    return <span key={`ellipsis-${i}`} className="w-8 h-8 flex items-center justify-center text-on-surface-variant opacity-40">...</span>;
                  }
                  const isActive = currentPage === p;
                  return (
                    <button 
                      key={p} 
                      onClick={() => setCurrentPage(Number(p))}
                      className={`w-8 h-8 flex items-center justify-center rounded-md text-sm transition-all ${isActive ? 'bg-primary text-white font-bold' : 'text-on-surface-variant opacity-60 hover:bg-surface-container font-medium'}`}
                    >
                      {p}
                    </button>
                  );
                });
              })()}

              <button 
                disabled={currentPage === totalPages}
                onClick={() => setCurrentPage(prev => Math.min(totalPages, prev + 1))}
                className={`w-8 h-8 flex items-center justify-center rounded-md transition-colors ${currentPage === totalPages ? 'text-on-surface-variant opacity-20 cursor-not-allowed' : 'text-on-surface-variant opacity-60 hover:bg-surface-container'}`}
              >
                <ChevronRight className="w-4 h-4" />
              </button>
            </div>
            <div className="flex items-center space-x-3">
              <span className="text-xs text-on-surface-variant opacity-40 font-medium">Mỗi trang</span>
              <div className="relative">
                <select 
                  value={pageSize}
                  onChange={(e) => {
                    const newSize = Number(e.target.value);
                    setPageSize(newSize);
                    setCurrentPage(1); // Reset to page 1 to avoid out of bounds
                  }}
                  className="appearance-none pl-3 pr-8 py-1.5 bg-surface-container-low border border-outline-variant rounded-lg text-xs font-bold text-on-surface focus:outline-none focus:ring-2 focus:ring-primary/20 transition-all cursor-pointer"
                >
                  <option value={10}>10</option>
                  <option value={20}>20</option>
                  <option value={50}>50</option>
                </select>
                <ChevronDown className="absolute right-2 top-1/2 -translate-y-1/2 w-3 h-3 text-on-surface-variant opacity-40 pointer-events-none" />
              </div>
            </div>
          </div>
        </section>

        {/* WxSkeleton Section */}
        <section className="space-y-6">
          <div className="flex items-center space-x-2 text-primary font-bold text-lg">
            <Bolt className="w-5 h-5" />
            <span>WxSkeleton</span>
          </div>
          <div className="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant transition-colors duration-300">
            <div className="flex items-start space-x-6">
              <div className="w-16 h-16 bg-surface-container rounded-full animate-pulse shrink-0" />
              <div className="flex-1 space-y-4">
                <div className="h-3 bg-surface-container rounded-full w-3/4 animate-pulse" />
                <div className="h-3 bg-surface-container rounded-full w-full animate-pulse" />
                <div className="h-3 bg-surface-container rounded-full w-5/6 animate-pulse" />
              </div>
            </div>
          </div>
        </section>

        {/* Cards Section */}
        <section className="space-y-6">
          <div className="flex items-center space-x-2 text-primary font-bold text-lg">
            <Bolt className="w-5 h-5" />
            <span>Cards</span>
          </div>
          <div className="bg-primary/10 p-8 rounded-2xl grid grid-cols-1 md:grid-cols-3 gap-8 transition-colors duration-300">
            <div className="glass-heavy p-6 rounded-2xl space-y-4 border border-white/20">
              <div className="w-12 h-12 bg-primary rounded-xl flex items-center justify-center text-white shadow-lg shadow-primary/30">
                <Bolt className="w-7 h-7" />
              </div>
              <h4 className="font-bold text-on-surface">Glass Heavy</h4>
              <p className="text-xs text-on-surface-variant leading-relaxed font-medium">Glassmorphism card, kiểu Login page.</p>
            </div>
            
            <div className="glass-medium p-6 rounded-2xl space-y-4 border border-white/10">
              <div className="w-12 h-12 bg-primary rounded-xl flex items-center justify-center text-white shadow-lg shadow-primary/30">
                <Bolt className="w-7 h-7" />
              </div>
              <h4 className="font-bold text-on-surface">Glass Medium</h4>
              <p className="text-xs text-on-surface-variant leading-relaxed font-medium">Toolbar / floating panel.</p>
            </div>

            <div className="glass-light p-6 rounded-2xl space-y-4 border border-white/5">
              <div className="flex items-center space-x-2 text-white">
                <Bolt className="w-6 h-6" />
                <span className="font-black text-sm tracking-widest uppercase">WEMAKE</span>
              </div>
              <h4 className="font-bold text-white">Glass Light</h4>
              <p className="text-xs text-white/90 leading-relaxed font-medium">Overlay / badge nền.</p>
            </div>
          </div>
        </section>

        {/* WxEmptyState Section */}
        <section className="space-y-6">
          <div className="flex items-center space-x-2 text-primary font-bold text-lg">
            <Bolt className="w-5 h-5" />
            <span>WxEmptyState</span>
          </div>
          <div className="bg-surface-container-lowest p-20 rounded-xl shadow-sm border border-outline-variant flex flex-col items-center justify-center space-y-6 transition-colors duration-300">
            <div className="w-24 h-24 text-primary opacity-20">
              <Bolt className="w-full h-full" strokeWidth={1} />
            </div>
            <p className="text-sm text-on-surface-variant font-medium tracking-wide">Không có dữ liệu</p>
          </div>
        </section>
          </>
        )}

        {activePage === 'patterns' && (
          <div className="space-y-12 pb-20">
            {/* Patterns Page Header */}
            <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
              <div>
                <div className="flex items-center space-x-2 text-xs font-bold text-on-surface-variant opacity-60 uppercase tracking-widest mb-1">
                  <span>Hệ thống</span>
                  <ChevronRight className="w-3 h-3" />
                  <span className="text-primary">Patterns</span>
                </div>
                <h2 className="text-2xl font-black text-on-surface tracking-tight">Thư viện Patterns & Components</h2>
                <p className="text-sm text-on-surface-variant mt-1">Các khối giao diện mẫu chuẩn hóa cho hệ thống Wemake.</p>
              </div>
              <div className="flex items-center space-x-3">
                <button className="px-4 py-2 bg-surface-container-lowest border border-outline-variant rounded-lg text-sm font-bold text-on-surface-variant hover:bg-surface-container transition-colors flex items-center space-x-2">
                  <FileText className="w-4 h-4" />
                  <span>Tài liệu</span>
                </button>
                <button className="px-5 py-2 btn-gradient text-white rounded-lg text-sm font-bold shadow-lg shadow-primary/20 flex items-center space-x-2">
                  <Plus className="w-4 h-4" />
                  <span>Thêm mẫu mới</span>
                </button>
              </div>
            </div>

            {/* 1. WxDataTable Section */}
            <section className="space-y-8">
              <div className="flex items-center space-x-2 text-primary font-bold text-xl">
                <Layout className="w-6 h-6" />
                <span>WxDataTable</span>
              </div>

              {/* 1.1. Bảng TKQC */}
              <div className="space-y-4">
                <h4 className="text-xs font-bold text-on-surface-variant dark:text-white opacity-60 dark:opacity-100 uppercase tracking-widest">BẢNG TKQC — CƠ BẢN VỚI STATUS BADGE</h4>
                <div className="bg-surface-container-lowest rounded-xl border border-outline-variant shadow-sm overflow-hidden transition-colors duration-300">
                  <table className="w-full text-left border-collapse">
                    <thead>
                      <tr className="bg-surface-container border-b border-outline-variant transition-colors duration-300">
                        <th 
                          className={`px-6 py-4 text-[10px] font-bold uppercase tracking-wider cursor-pointer transition-colors hover:bg-surface-container-low ${sortConfig.key === 'id' ? 'text-primary' : 'text-on-surface-variant opacity-60'}`}
                          onClick={() => handleSort('id')}
                        >
                          ID {getSortIcon('id')}
                        </th>
                        <th 
                          className={`px-6 py-4 text-[10px] font-bold uppercase tracking-wider cursor-pointer transition-colors hover:bg-surface-container-low ${sortConfig.key === 'name' ? 'text-primary' : 'text-on-surface-variant opacity-60'}`}
                          onClick={() => handleSort('name')}
                        >
                          TÊN TÀI KHOẢN {getSortIcon('name')}
                        </th>
                        <th 
                          className={`px-6 py-4 text-[10px] font-bold uppercase tracking-wider text-center cursor-pointer transition-colors hover:bg-surface-container-low ${sortConfig.key === 'status' ? 'text-primary' : 'text-on-surface-variant opacity-60'}`}
                          onClick={() => handleSort('status')}
                        >
                          TRẠNG THÁI {getSortIcon('status')}
                        </th>
                        <th 
                          className={`px-6 py-4 text-[10px] font-bold uppercase tracking-wider text-right cursor-pointer transition-colors hover:bg-surface-container-low ${sortConfig.key === 'spend' ? 'text-primary' : 'text-on-surface-variant opacity-60'}`}
                          onClick={() => handleSort('spend')}
                        >
                          CHI TIÊU {getSortIcon('spend')}
                        </th>
                        <th 
                          className={`px-6 py-4 text-[10px] font-bold uppercase tracking-wider text-right cursor-pointer transition-colors hover:bg-surface-container-low ${sortConfig.key === 'updated' ? 'text-primary' : 'text-on-surface-variant opacity-60'}`}
                          onClick={() => handleSort('updated')}
                        >
                          CẬP NHẬT {getSortIcon('updated')}
                        </th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-outline-variant">
                      {tableData.map((row) => (
                        <tr key={row.id} className="hover:bg-surface-container/50 transition-colors">
                          <td className="px-6 py-4 text-xs text-on-surface-variant font-mono">{row.id}</td>
                          <td className="px-6 py-4 text-xs font-bold text-on-surface">{row.name}</td>
                          <td className="px-6 py-4 text-center">
                            <span className={`px-2 py-0.5 rounded-full text-[9px] font-black border ${
                              row.status === 'ACTIVE' ? 'bg-emerald-500/10 text-emerald-500 border-emerald-500/20' :
                              row.status === 'PAUSED' ? 'bg-amber-500/10 text-amber-500 border-amber-500/20' :
                              row.status === 'ERROR' ? 'bg-rose-500/10 text-rose-500 border-rose-500/20' :
                              'bg-surface-container text-on-surface-variant border-outline-variant'
                            }`}>
                              ● {row.status}
                            </span>
                          </td>
                          <td className="px-6 py-4 text-xs font-bold text-on-surface text-right">{row.spend}</td>
                          <td className="px-6 py-4 text-xs text-on-surface-variant text-right">{row.updated}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              </div>

              {/* 1.2. Bảng BM */}
              <div className="space-y-4">
                <div className="flex justify-between items-end">
                  <h4 className="text-xs font-bold text-on-surface-variant dark:text-white opacity-60 dark:opacity-100 uppercase tracking-widest">BẢNG BM — AVATAR + CHECKBOX + STATUS</h4>
                  <div className="flex items-center space-x-2 bg-surface-container p-1 rounded-lg border border-outline-variant transition-colors duration-300">
                    {['ALL', 'VERIFIED', 'RESTRICTED', 'UNDER REVIEW'].map((status) => (
                      <button
                        key={status}
                        onClick={() => setBMStatusFilter(status)}
                        className={`px-3 py-1 rounded-md text-[10px] font-bold transition-all ${
                          bmStatusFilter === status 
                            ? 'bg-surface-container-lowest text-primary shadow-sm border border-outline-variant' 
                            : 'text-on-surface-variant opacity-60 hover:text-on-surface'
                        }`}
                      >
                        {status}
                      </button>
                    ))}
                  </div>
                </div>
                <div className="bg-surface-container-lowest rounded-xl border border-outline-variant shadow-sm overflow-hidden transition-colors duration-300">
                  <table className="w-full text-left border-collapse">
                    <thead>
                      <tr className="bg-surface-container border-b border-outline-variant transition-colors duration-300">
                        <th className="px-6 py-4 w-12">
                          <button 
                            onClick={toggleBMSelectAll}
                            className={`w-4 h-4 border-2 rounded-sm transition-colors flex items-center justify-center ${
                              selectedBMRows.length === bmData.length 
                                ? 'bg-primary border-primary' 
                                : 'border-outline-variant'
                            }`}
                          >
                            {selectedBMRows.length === bmData.length && <Check className="w-3 h-3 text-white" />}
                          </button>
                        </th>
                        <th 
                          className={`px-6 py-4 text-[10px] font-bold uppercase tracking-wider cursor-pointer transition-colors hover:bg-surface-container-low ${bmSortConfig.key === 'id' ? 'text-primary' : 'text-on-surface-variant opacity-60'}`}
                          onClick={() => handleBMSort('id')}
                        >
                          BM ID {getBMSortIcon('id')}
                        </th>
                        <th 
                          className={`px-6 py-4 text-[10px] font-bold uppercase tracking-wider cursor-pointer transition-colors hover:bg-surface-container-low ${bmSortConfig.key === 'name' ? 'text-primary' : 'text-on-surface-variant opacity-60'}`}
                          onClick={() => handleBMSort('name')}
                        >
                          TÊN BM {getBMSortIcon('name')}
                        </th>
                        <th 
                          className={`px-6 py-4 text-[10px] font-bold uppercase tracking-wider text-center cursor-pointer transition-colors hover:bg-surface-container-low ${bmSortConfig.key === 'count' ? 'text-primary' : 'text-on-surface-variant opacity-60'}`}
                          onClick={() => handleBMSort('count')}
                        >
                          SỐ TKQC {getBMSortIcon('count')}
                        </th>
                        <th 
                          className={`px-6 py-4 text-[10px] font-bold uppercase tracking-wider text-center cursor-pointer transition-colors hover:bg-surface-container-low ${bmSortConfig.key === 'status' ? 'text-primary' : 'text-on-surface-variant opacity-60'}`}
                          onClick={() => handleBMSort('status')}
                        >
                          TÌNH TRẠNG {getBMSortIcon('status')}
                        </th>
                        <th 
                          className={`px-6 py-4 text-[10px] font-bold uppercase tracking-wider text-right cursor-pointer transition-colors hover:bg-surface-container-low ${bmSortConfig.key === 'limit' ? 'text-primary' : 'text-on-surface-variant opacity-60'}`}
                          onClick={() => handleBMSort('limit')}
                        >
                          LIMIT CHI TIÊU {getBMSortIcon('limit')}
                        </th>
                        <th 
                          className={`px-6 py-4 text-[10px] font-bold uppercase tracking-wider text-right cursor-pointer transition-colors hover:bg-surface-container-low ${bmSortConfig.key === 'manager' ? 'text-primary' : 'text-on-surface-variant opacity-60'}`}
                          onClick={() => handleBMSort('manager')}
                        >
                          NGƯỜI QUẢN LÝ {getBMSortIcon('manager')}
                        </th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-outline-variant">
                      {filteredBMData.map((row) => (
                        <tr key={row.id} className={`hover:bg-surface-container/50 transition-colors ${selectedBMRows.includes(row.id) ? 'bg-primary/5' : ''}`}>
                          <td className="px-6 py-4">
                            <button 
                              onClick={() => toggleBMSelectRow(row.id)}
                              className={`w-4 h-4 border-2 rounded-sm transition-colors flex items-center justify-center ${
                                selectedBMRows.includes(row.id)
                                  ? 'bg-primary border-primary' 
                                  : 'border-outline-variant'
                              }`}
                            >
                              {selectedBMRows.includes(row.id) && <Check className="w-3 h-3 text-white" />}
                            </button>
                          </td>
                          <td className="px-6 py-4 text-xs text-on-surface-variant opacity-60 font-mono">{row.id}</td>
                          <td className="px-6 py-4">
                            <div className="flex items-center space-x-3">
                              <div className="w-8 h-8 rounded-lg bg-primary/10 flex items-center justify-center">
                                <Layers className="w-4 h-4 text-primary" />
                              </div>
                              <div>
                                <p className="text-xs font-bold text-on-surface">{row.name}</p>
                                <p className="text-[10px] text-on-surface-variant opacity-60">{row.id}</p>
                              </div>
                            </div>
                          </td>
                          <td className="px-6 py-4 text-center">
                            <span className="px-2 py-0.5 bg-primary/10 text-primary rounded-full text-[10px] font-black">{row.count}</span>
                          </td>
                          <td className="px-6 py-4 text-center">
                            <span className={`px-2 py-0.5 rounded-full text-[9px] font-black border ${
                              row.color === 'teal' ? 'bg-emerald-500/10 text-emerald-500 border-emerald-500/20' :
                              row.color === 'rose' ? 'bg-rose-500/10 text-rose-500 border-rose-500/20' :
                              'bg-amber-500/10 text-amber-500 border-amber-500/20'
                            }`}>
                              ● {row.status}
                            </span>
                          </td>
                          <td className="px-6 py-4 text-xs font-bold text-emerald-500 text-right">{row.limit}</td>
                          <td className="px-6 py-4 text-xs text-on-surface-variant text-right">{row.manager}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              </div>

              {/* 1.3. Nhật ký hoạt động */}
              <div className="space-y-4">
                <div className="flex justify-between items-end">
                  <h4 className="text-xs font-bold text-on-surface-variant dark:text-white opacity-60 dark:opacity-100 uppercase tracking-widest">NHẬT KÝ HOẠT ĐỘNG — COMPACT, COLOR-CODED ACTIONS</h4>
                  <div className="flex items-center space-x-2 bg-surface-container p-1 rounded-lg border border-outline-variant transition-colors duration-300">
                    {['ALL', 'Đăng nhập', 'Tạo chiến dịch', 'Chỉnh sửa TKQC', 'Xuất báo cáo', 'Xóa tài khoản'].map((action) => (
                      <button
                        key={action}
                        onClick={() => setLogActionFilter(action)}
                        className={`px-3 py-1 rounded-md text-[10px] font-bold transition-all ${
                          logActionFilter === action 
                            ? 'bg-surface-container-lowest text-primary shadow-sm border border-outline-variant' 
                            : 'text-on-surface-variant opacity-60 hover:text-on-surface'
                        }`}
                      >
                        {action}
                      </button>
                    ))}
                  </div>
                </div>
                <div className="bg-surface-container-lowest rounded-xl border border-outline-variant shadow-sm overflow-hidden transition-colors duration-300">
                  <table className="w-full text-left border-collapse">
                    <thead>
                      <tr className="bg-surface-container border-b border-outline-variant transition-colors duration-300">
                        <th 
                          className={`px-6 py-3 text-[10px] font-bold uppercase tracking-wider cursor-pointer transition-colors hover:bg-surface-container-low ${logSortConfig.key === 'time' ? 'text-primary' : 'text-on-surface-variant opacity-60'}`}
                          onClick={() => handleLogSort('time')}
                        >
                          THỜI GIAN {getLogSortIcon('time')}
                        </th>
                        <th 
                          className={`px-6 py-3 text-[10px] font-bold uppercase tracking-wider cursor-pointer transition-colors hover:bg-surface-container-low ${logSortConfig.key === 'user' ? 'text-primary' : 'text-on-surface-variant opacity-60'}`}
                          onClick={() => handleLogSort('user')}
                        >
                          NGƯỜI DÙNG {getLogSortIcon('user')}
                        </th>
                        <th 
                          className={`px-6 py-3 text-[10px] font-bold uppercase tracking-wider text-center cursor-pointer transition-colors hover:bg-surface-container-low ${logSortConfig.key === 'action' ? 'text-primary' : 'text-on-surface-variant opacity-60'}`}
                          onClick={() => handleLogSort('action')}
                        >
                          HÀNH ĐỘNG {getLogSortIcon('action')}
                        </th>
                        <th 
                          className={`px-6 py-3 text-[10px] font-bold uppercase tracking-wider cursor-pointer transition-colors hover:bg-surface-container-low ${logSortConfig.key === 'object' ? 'text-primary' : 'text-on-surface-variant opacity-60'}`}
                          onClick={() => handleLogSort('object')}
                        >
                          ĐỐI TƯỢNG {getLogSortIcon('object')}
                        </th>
                        <th 
                          className={`px-6 py-3 text-[10px] font-bold uppercase tracking-wider text-right cursor-pointer transition-colors hover:bg-surface-container-low ${logSortConfig.key === 'ip' ? 'text-primary' : 'text-on-surface-variant opacity-60'}`}
                          onClick={() => handleLogSort('ip')}
                        >
                          IP {getLogSortIcon('ip')}
                        </th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-outline-variant">
                      {filteredLogData.map((row, i) => (
                        <tr key={i} className="hover:bg-surface-container/50 transition-colors">
                          <td className="px-6 py-3 text-[10px] text-on-surface-variant opacity-60 font-mono bg-surface-container/30">{row.time}</td>
                          <td className="px-6 py-3">
                            <div className="flex items-center space-x-2">
                              <div className="w-6 h-6 rounded-full bg-primary/10 flex items-center justify-center text-[8px] font-black text-primary uppercase">
                                {row.user[0]}
                              </div>
                              <span className="text-[10px] font-bold text-on-surface">{row.user}</span>
                            </div>
                          </td>
                          <td className="px-6 py-3 text-center">
                            <span className={`px-2 py-0.5 rounded text-[9px] font-black border ${
                              row.color === 'sky' ? 'bg-sky-500/10 text-sky-500 border-sky-500/20' :
                              row.color === 'teal' ? 'bg-emerald-500/10 text-emerald-500 border-emerald-500/20' :
                              row.color === 'amber' ? 'bg-amber-500/10 text-amber-500 border-amber-500/20' :
                              row.color === 'rose' ? 'bg-rose-500/10 text-rose-500 border-rose-500/20' :
                              'bg-surface-container text-on-surface-variant border-outline-variant'
                            }`}>
                              {row.action}
                            </span>
                          </td>
                          <td className="px-6 py-3 text-[10px] font-bold text-on-surface-variant opacity-80">{row.object}</td>
                          <td className="px-6 py-3 text-[10px] text-on-surface-variant opacity-40 text-right font-mono">{row.ip}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              </div>

              {/* 1.4. Thành viên */}
              <div className="space-y-4">
                <div className="flex justify-between items-end">
                  <h4 className="text-xs font-bold text-on-surface-variant dark:text-white opacity-60 dark:opacity-100 uppercase tracking-widest">THÀNH VIÊN — AVATAR, VAI TRÒ, TRẠNG THÁI ONLINE</h4>
                  <div className="flex items-center space-x-4">
                    <div className="flex items-center space-x-2 bg-surface-container p-1 rounded-lg border border-outline-variant transition-colors duration-300">
                      {['ALL', 'Admin', 'Editor', 'Developer', 'Viewer'].map((role) => (
                        <button
                          key={role}
                          onClick={() => setMemberRoleFilter(role)}
                          className={`px-3 py-1 rounded-md text-[10px] font-bold transition-all ${
                            memberRoleFilter === role 
                              ? 'bg-surface-container-lowest text-primary shadow-sm border border-outline-variant' 
                              : 'text-on-surface-variant opacity-60 hover:text-on-surface'
                          }`}
                        >
                          {role}
                        </button>
                      ))}
                    </div>
                    <div className="flex items-center space-x-2 bg-surface-container p-1 rounded-lg border border-outline-variant transition-colors duration-300">
                      {['ALL', 'Online', 'Offline'].map((status) => (
                        <button
                          key={status}
                          onClick={() => setMemberStatusFilter(status)}
                          className={`px-3 py-1 rounded-md text-[10px] font-bold transition-all ${
                            memberStatusFilter === status 
                              ? 'bg-surface-container-lowest text-primary shadow-sm border border-outline-variant' 
                              : 'text-on-surface-variant opacity-60 hover:text-on-surface'
                          }`}
                        >
                          {status}
                        </button>
                      ))}
                    </div>
                  </div>
                </div>
                <div className="bg-surface-container-lowest rounded-xl border border-outline-variant shadow-sm overflow-hidden transition-colors duration-300">
                  <table className="w-full text-left border-collapse">
                    <thead>
                      <tr className="bg-surface-container border-b border-outline-variant transition-colors duration-300">
                        <th 
                          className={`px-6 py-4 text-[10px] font-bold uppercase tracking-wider cursor-pointer transition-colors hover:bg-surface-container-low ${memberSortConfig.key === 'name' ? 'text-primary' : 'text-on-surface-variant opacity-60'}`}
                          onClick={() => handleMemberSort('name')}
                        >
                          HỌ TÊN {getMemberSortIcon('name')}
                        </th>
                        <th 
                          className={`px-6 py-4 text-[10px] font-bold uppercase tracking-wider cursor-pointer transition-colors hover:bg-surface-container-low ${memberSortConfig.key === 'email' ? 'text-primary' : 'text-on-surface-variant opacity-60'}`}
                          onClick={() => handleMemberSort('email')}
                        >
                          EMAIL {getMemberSortIcon('email')}
                        </th>
                        <th 
                          className={`px-6 py-4 text-[10px] font-bold uppercase tracking-wider text-center cursor-pointer transition-colors hover:bg-surface-container-low ${memberSortConfig.key === 'role' ? 'text-primary' : 'text-on-surface-variant opacity-60'}`}
                          onClick={() => handleMemberSort('role')}
                        >
                          VAI TRÒ {getMemberSortIcon('role')}
                        </th>
                        <th 
                          className={`px-6 py-4 text-[10px] font-bold uppercase tracking-wider text-right cursor-pointer transition-colors hover:bg-surface-container-low ${memberSortConfig.key === 'last' ? 'text-primary' : 'text-on-surface-variant opacity-60'}`}
                          onClick={() => handleMemberSort('last')}
                        >
                          ĐĂNG NHẬP CUỐI {getMemberSortIcon('last')}
                        </th>
                        <th 
                          className={`px-6 py-4 text-[10px] font-bold uppercase tracking-wider text-right cursor-pointer transition-colors hover:bg-surface-container-low ${memberSortConfig.key === 'online' ? 'text-primary' : 'text-on-surface-variant opacity-60'}`}
                          onClick={() => handleMemberSort('online')}
                        >
                          TRẠNG THÁI {getMemberSortIcon('online')}
                        </th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-outline-variant">
                      {filteredMemberData.map((row, i) => (
                        <tr key={i} className="hover:bg-surface-container/50 transition-colors">
                          <td className="px-6 py-4">
                            <div className="flex items-center space-x-3">
                              <div className={`w-9 h-9 rounded-full flex items-center justify-center text-xs font-black text-white ${
                                i % 3 === 0 ? 'bg-sky-500' : i % 3 === 1 ? 'bg-emerald-500' : 'bg-purple-500'
                              }`}>
                                {row.name.split(' ').map(n => n[0]).join('')}
                              </div>
                              <div>
                                <p className="text-xs font-bold text-on-surface">{row.name}</p>
                                <p className="text-[10px] text-on-surface-variant opacity-60">{row.email}</p>
                              </div>
                            </div>
                          </td>
                          <td className="px-6 py-4 text-xs text-on-surface-variant opacity-80">{row.email}</td>
                          <td className="px-6 py-4 text-center">
                            <span className={`px-2 py-0.5 rounded text-[9px] font-bold border ${
                              row.role === 'Admin' ? 'bg-blue-500/10 text-blue-500 border-blue-500/20' :
                              row.role === 'Editor' ? 'bg-sky-500/10 text-sky-500 border-sky-500/20' :
                              row.role === 'Developer' ? 'bg-emerald-500/10 text-emerald-500 border-emerald-500/20' :
                              'bg-surface-container text-on-surface-variant border-outline-variant'
                            }`}>
                              {row.role}
                            </span>
                          </td>
                          <td className="px-6 py-4 text-[10px] text-on-surface-variant opacity-60 text-right">{row.last}</td>
                          <td className="px-6 py-4 text-right">
                            <div className="flex items-center justify-end space-x-1.5">
                              <div className={`w-2 h-2 rounded-full ${row.online ? 'bg-emerald-500 animate-pulse' : 'bg-on-surface-variant opacity-30'}`} />
                              <span className={`text-[10px] font-bold ${row.online ? 'text-emerald-500' : 'text-on-surface-variant opacity-40'}`}>
                                {row.online ? 'Online' : 'Offline'}
                              </span>
                            </div>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              </div>

              {/* 1.5. Loading & Empty State */}
              <div className="space-y-4">
                <div className="flex justify-between items-center">
                  <h4 className="text-xs font-bold text-on-surface-variant opacity-60 uppercase tracking-widest">LOADING & EMPTY STATE</h4>
                  <div className="flex items-center space-x-2 bg-surface-container p-1 rounded-lg border border-outline-variant">
                    {['loading', 'empty', 'loaded'].map((state) => (
                      <button
                        key={state}
                        onClick={() => setDemoState(state as any)}
                        className={`px-3 py-1 rounded-md text-[10px] font-bold transition-all uppercase tracking-wider ${
                          demoState === state 
                            ? 'bg-surface-container-lowest text-primary shadow-sm border border-outline-variant' 
                            : 'text-on-surface-variant hover:text-on-surface'
                        }`}
                      >
                        {state === 'loading' ? 'Loading' : state === 'empty' ? 'Empty' : 'Loaded'}
                      </button>
                    ))}
                  </div>
                </div>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                  {/* Card 1 */}
                  <div className="bg-surface-container-lowest rounded-xl border border-outline-variant shadow-sm overflow-hidden h-48 flex flex-col transition-colors duration-300">
                    <div className="bg-surface-container border-b border-outline-variant px-6 py-3 flex gap-4">
                      <div className={`h-2 w-16 bg-outline-variant rounded ${demoState === 'loading' ? 'animate-pulse' : ''}`} />
                      <div className={`h-2 w-16 bg-outline-variant rounded ${demoState === 'loading' ? 'animate-pulse' : ''}`} />
                      <div className={`h-2 w-16 bg-outline-variant rounded ${demoState === 'loading' ? 'animate-pulse' : ''}`} />
                    </div>
                    <div className="flex-1 flex flex-col p-4">
                      {demoState === 'loading' ? (
                        <div className="flex-1 flex flex-col items-center justify-center space-y-3">
                          <Loader2 className="w-6 h-6 text-primary animate-spin" />
                          <p className="text-[10px] font-bold text-on-surface-variant opacity-60 uppercase tracking-wider">Đang tải dữ liệu...</p>
                        </div>
                      ) : demoState === 'empty' ? (
                        <div className="flex-1 flex flex-col items-center justify-center space-y-3">
                          <div className="w-10 h-10 rounded-full bg-surface-container flex items-center justify-center">
                            <FileText className="w-5 h-5 text-on-surface-variant opacity-40" />
                          </div>
                          <p className="text-[10px] font-bold text-on-surface-variant opacity-60 uppercase tracking-wider">Chưa có dữ liệu nào</p>
                        </div>
                      ) : (
                        <div className="space-y-3">
                          {[1, 2, 3].map((i) => (
                            <div key={i} className="flex items-center justify-between p-2 rounded-lg bg-surface-container border border-outline-variant">
                              <div className="flex items-center space-x-3">
                                <div className="w-8 h-8 rounded bg-surface-container-lowest border border-outline-variant flex items-center justify-center">
                                  <FileText className="w-4 h-4 text-primary" />
                                </div>
                                <div>
                                  <p className="text-[10px] font-bold text-on-surface">Tài liệu dự án #{i}</p>
                                  <p className="text-[9px] text-on-surface-variant opacity-60">2.4 MB • PDF</p>
                                </div>
                              </div>
                              <button className="p-1.5 hover:bg-surface-container-lowest rounded-md transition-colors">
                                <Download className="w-3 h-3 text-on-surface-variant opacity-60" />
                              </button>
                            </div>
                          ))}
                        </div>
                      )}
                    </div>
                  </div>

                  {/* Card 2 */}
                  <div className="bg-surface-container-lowest rounded-xl border border-outline-variant shadow-sm overflow-hidden h-48 flex flex-col transition-colors duration-300">
                    <div className="bg-surface-container border-b border-outline-variant px-6 py-3 flex gap-4">
                      <div className={`h-2 w-16 bg-outline-variant rounded ${demoState === 'loading' ? 'animate-pulse' : ''}`} />
                      <div className={`h-2 w-16 bg-outline-variant rounded ${demoState === 'loading' ? 'animate-pulse' : ''}`} />
                      <div className={`h-2 w-16 bg-outline-variant rounded ${demoState === 'loading' ? 'animate-pulse' : ''}`} />
                    </div>
                    <div className="flex-1 flex flex-col p-4">
                      {demoState === 'loading' ? (
                        <div className="flex-1 flex flex-col items-center justify-center space-y-3">
                          <Loader2 className="w-6 h-6 text-primary animate-spin" />
                          <p className="text-[10px] font-bold text-on-surface-variant opacity-60 uppercase tracking-wider">Đang tải dữ liệu...</p>
                        </div>
                      ) : demoState === 'empty' ? (
                        <div className="flex-1 flex flex-col items-center justify-center space-y-3">
                          <div className="w-10 h-10 rounded-full bg-surface-container flex items-center justify-center">
                            <FileText className="w-5 h-5 text-on-surface-variant opacity-40" />
                          </div>
                          <p className="text-[10px] font-bold text-on-surface-variant opacity-60 uppercase tracking-wider">Chưa có dữ liệu nào</p>
                        </div>
                      ) : (
                        <div className="space-y-3">
                          {[
                            { name: 'Nguyễn Văn A', role: 'Quản trị viên' },
                            { name: 'Trần Thị B', role: 'Biên tập viên' }
                          ].map((user, i) => (
                            <div key={i} className="flex items-center space-x-4 p-3 rounded-xl border border-outline-variant bg-surface-container shadow-sm">
                              <div className="w-10 h-10 rounded-full bg-primary/10 flex items-center justify-center">
                                <User className="w-5 h-5 text-primary" />
                              </div>
                              <div className="flex-1 min-w-0">
                                <p className="text-[10px] font-bold text-on-surface truncate">{user.name}</p>
                                <p className="text-[9px] text-on-surface-variant opacity-60 truncate">{user.role}</p>
                              </div>
                            </div>
                          ))}
                          <div className="flex-1 flex items-center justify-center pt-1">
                            <button className="text-[10px] font-bold text-primary hover:underline">Xem tất cả</button>
                          </div>
                        </div>
                      )}
                    </div>
                  </div>
                </div>
              </div>
            </section>

            {/* 2. WxStatCard Section */}
            <section className="space-y-6">
              <div className="flex items-center space-x-2 text-primary font-bold text-xl">
                <Activity className="w-6 h-6" />
                <span>WxStatCard</span>
              </div>
              <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                {[
                  { label: 'TỔNG TÀI KHOẢN', value: '1.234', change: '+12%', sub: 'so với tháng trước', icon: Users, color: 'bg-sky-500' },
                  { label: 'CHI TIÊU HÔM NAY', value: '$5,678.90', change: '-3.2%', sub: 'so với hôm qua', icon: Coins, color: 'bg-emerald-500' },
                  { label: 'LỖI PHÁT HIỆN', value: '7', change: '0%', sub: 'so với tháng trước', icon: AlertCircle, color: 'bg-rose-500' },
                ].map((stat, i) => (
                  <div key={i} className="bg-white p-6 rounded-2xl border border-slate-100 shadow-sm flex items-center space-x-6">
                    <div className={`w-14 h-14 rounded-2xl ${stat.color} flex items-center justify-center shadow-lg shadow-primary/20`}>
                      <stat.icon className="w-7 h-7 text-white" />
                    </div>
                    <div className="space-y-1">
                      <p className="text-[10px] font-black text-slate-400 tracking-widest">{stat.label}</p>
                      <h3 className="text-2xl font-black text-slate-800">{stat.value}</h3>
                      <div className="flex items-center space-x-2">
                        <span className={`text-[10px] font-black px-1.5 py-0.5 rounded ${
                          stat.change.startsWith('+') ? 'bg-emerald-50 text-emerald-600' : 
                          stat.change.startsWith('-') ? 'bg-rose-50 text-rose-600' : 'bg-slate-50 text-slate-500'
                        }`}>
                          {stat.change}
                        </span>
                        <span className="text-[10px] text-slate-400 font-medium">{stat.sub}</span>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            </section>

            {/* 3. WxAlert Section */}
            <section className="space-y-6">
              <div className="flex items-center space-x-2 text-primary font-bold text-xl">
                <Bell className="w-6 h-6" />
                <span>WxAlert</span>
              </div>
              <div className="space-y-3">
                <div className="p-4 bg-sky-50 border border-sky-100 rounded-xl flex items-start space-x-3 relative group">
                  <Info className="w-5 h-5 text-sky-500 mt-0.5" />
                  <div>
                    <h5 className="text-sm font-bold text-sky-800">Thông tin</h5>
                    <p className="text-xs text-sky-600 mt-0.5">Hệ thống sẽ bảo trì lúc 2:00 AM ngày 23/03.</p>
                  </div>
                  <button className="absolute right-4 top-4 text-sky-400 hover:text-sky-600 opacity-0 group-hover:opacity-100 transition-opacity"><X className="w-4 h-4" /></button>
                </div>
                <div className="p-4 bg-emerald-50 border border-emerald-100 rounded-xl flex items-start space-x-3 relative group">
                  <CheckCircle2 className="w-5 h-5 text-emerald-500 mt-0.5" />
                  <div>
                    <h5 className="text-sm font-bold text-emerald-800">Thành công</h5>
                    <p className="text-xs text-emerald-600 mt-0.5">Đã import 1,234 tài khoản vào hệ thống.</p>
                  </div>
                  <button className="absolute right-4 top-4 text-emerald-400 hover:text-emerald-600 opacity-0 group-hover:opacity-100 transition-opacity"><X className="w-4 h-4" /></button>
                </div>
                <div className="p-4 bg-amber-50 border border-amber-100 rounded-xl flex items-start space-x-3 relative group">
                  <AlertTriangle className="w-5 h-5 text-amber-500 mt-0.5" />
                  <div>
                    <h5 className="text-sm font-bold text-amber-800">Cảnh báo</h5>
                    <p className="text-xs text-amber-600 mt-0.5">Proxy sắp hết hạn trong 3 ngày.</p>
                  </div>
                  <button className="absolute right-4 top-4 text-amber-400 hover:text-amber-600 opacity-0 group-hover:opacity-100 transition-opacity"><X className="w-4 h-4" /></button>
                </div>
                <div className="p-4 bg-rose-50 border border-rose-100 rounded-xl flex items-start space-x-3 relative group">
                  <AlertCircle className="w-5 h-5 text-rose-500 mt-0.5" />
                  <div>
                    <h5 className="text-sm font-bold text-rose-800">Lỗi</h5>
                    <p className="text-xs text-rose-600 mt-0.5">Không thể kết nối tới API Facebook. Vui lòng kiểm tra lại proxy.</p>
                  </div>
                  <button className="absolute right-4 top-4 text-rose-400 hover:text-rose-600 opacity-0 group-hover:opacity-100 transition-opacity"><X className="w-4 h-4" /></button>
                </div>
              </div>
            </section>

            {/* 4. WxSettingSection + WxToggleRow Section */}
            <section className="space-y-6">
              <div className="flex items-center space-x-2 text-primary font-bold text-xl">
                <Settings className="w-6 h-6" />
                <span>WxSettingSection + WxToggleRow</span>
              </div>
              <div className="bg-surface-container-lowest p-8 rounded-2xl border border-outline-variant shadow-sm space-y-8 transition-colors duration-300">
                <div>
                  <h4 className="text-lg font-black text-on-surface tracking-tight uppercase">CÀI ĐẶT CHỨC NĂNG</h4>
                  <p className="text-sm text-on-surface-variant">Các tùy chọn cấu hình cho ứng dụng</p>
                </div>
                <div className="space-y-6">
                  {[
                    { id: 'autoDownload', label: 'Tự động tải dữ liệu', sub: 'Tải dữ liệu ngay khi khởi động tiện ích', icon: CloudUpload },
                    { id: 'showNotifications', label: 'Hiển thị thông báo', sub: 'Hiển thị popup thông báo khi có cập nhật', icon: Bell },
                    { id: 'syncBackground', label: 'Đồng bộ nền', sub: 'Duy trì kết nối và đồng bộ dữ liệu khi ẩn ứng dụng', icon: RefreshCw },
                    { id: 'loginProtection', label: 'Bảo vệ đăng nhập', sub: 'Yêu cầu 2FA khi truy cập các mục nhạy cảm', icon: Lock },
                  ].map((item) => (
                    <div key={item.id} className="flex items-center justify-between group">
                      <div className="flex items-center space-x-4">
                        <div className="w-10 h-10 rounded-xl bg-surface-container flex items-center justify-center group-hover:bg-primary/10 transition-colors">
                          <item.icon className="w-5 h-5 text-on-surface-variant group-hover:text-primary transition-colors" />
                        </div>
                        <div>
                          <p className="text-sm font-bold text-on-surface">{item.label}</p>
                          <p className="text-xs text-on-surface-variant">{item.sub}</p>
                        </div>
                      </div>
                      <button 
                        onClick={() => setPatternsToggles(prev => ({ ...prev, [item.id]: !prev[item.id as keyof typeof patternsToggles] }))}
                        className={`w-12 h-6 rounded-full transition-all relative ${patternsToggles[item.id as keyof typeof patternsToggles] ? 'bg-primary' : 'bg-surface-container-high'}`}
                      >
                        <div className={`absolute top-1 w-4 h-4 bg-white rounded-full transition-all ${patternsToggles[item.id as keyof typeof patternsToggles] ? 'left-7' : 'left-1'}`} />
                      </button>
                    </div>
                  ))}
                </div>
              </div>
            </section>

            {/* 5. WxOptionCard Section */}
            <section className="space-y-6">
              <div className="flex items-center space-x-2 text-primary font-bold text-xl">
                <CheckCircle2 className="w-6 h-6" />
                <span>WxOptionCard</span>
              </div>
              <div className="space-y-4">
                {[
                  { id: 'option1', title: 'Phương thức 1', sub: 'Mặc định', desc: 'Sử dụng cấu hình tiêu chuẩn cho mọi tài khoản.' },
                  { id: 'option2', title: 'Phương thức 2', sub: 'Nhanh hơn', desc: 'Tối ưu hóa tốc độ tải dữ liệu nhưng tốn nhiều tài nguyên hơn.' },
                  { id: 'option3', title: 'Phương thức 3', sub: 'Bảo mật cao', desc: 'Mã hóa toàn bộ dữ liệu truyền tải, yêu cầu 2FA.', disabled: true },
                ].map((opt) => (
                  <button 
                    key={opt.id}
                    disabled={opt.disabled}
                    onClick={() => setSelectedOption(opt.id)}
                    className={`w-full text-left p-5 rounded-2xl border-2 transition-all flex items-center justify-between ${
                      selectedOption === opt.id ? 'border-primary bg-primary/10' : 'border-outline-variant bg-surface-container-lowest hover:border-outline'
                    } ${opt.disabled ? 'opacity-50 cursor-not-allowed' : ''}`}
                  >
                    <div className="flex items-center space-x-4">
                      <div className={`w-5 h-5 rounded-full border-2 flex items-center justify-center ${selectedOption === opt.id ? 'border-primary' : 'border-on-surface-variant'}`}>
                        {selectedOption === opt.id && <div className="w-2.5 h-2.5 bg-primary rounded-full" />}
                      </div>
                      <div>
                        <div className="flex items-center space-x-2">
                          <span className="text-sm font-bold text-on-surface">{opt.title}</span>
                          <span className="text-[10px] font-bold text-on-surface-variant uppercase">{opt.sub}</span>
                        </div>
                        <p className="text-xs text-on-surface-variant mt-0.5">{opt.desc}</p>
                      </div>
                    </div>
                    {opt.disabled && <span className="text-[10px] font-bold text-on-surface-variant bg-surface-container px-2 py-0.5 rounded">Cần 2FA</span>}
                  </button>
                ))}
              </div>
            </section>

            {/* 6. WxCollapsiblePanel Section */}
            <section className="space-y-6">
              <div className="flex items-center space-x-2 text-primary font-bold text-xl">
                <ChevronDown className="w-6 h-6" />
                <span>WxCollapsiblePanel</span>
              </div>
              <div className="space-y-4">
                {[
                  { id: 'panel1', title: 'Advanced Option 1', icon: List },
                  { id: 'panel2', title: 'Advanced Option 2', icon: Settings },
                ].map((panel) => (
                  <div key={panel.id} className="bg-surface-container-lowest rounded-2xl border border-outline-variant shadow-sm overflow-hidden transition-colors duration-300">
                    <button 
                      onClick={() => setExpandedPanels(prev => prev.includes(panel.id) ? prev.filter(p => p !== panel.id) : [...prev, panel.id])}
                      className="w-full px-6 py-4 flex items-center justify-between hover:bg-surface-container transition-colors"
                    >
                      <div className="flex items-center space-x-3">
                        <panel.icon className="w-4 h-4 text-on-surface-variant" />
                        <span className="text-sm font-bold text-on-surface">{panel.title}</span>
                      </div>
                      <div className="flex items-center space-x-4">
                        <div className="w-10 h-5 bg-surface-container rounded-full relative">
                          <div className="absolute left-1 top-1 w-3 h-3 bg-white rounded-full" />
                        </div>
                        <ChevronDown className={`w-4 h-4 text-on-surface-variant transition-transform ${expandedPanels.includes(panel.id) ? 'rotate-180' : ''}`} />
                      </div>
                    </button>
                    <AnimatePresence>
                      {expandedPanels.includes(panel.id) && (
                        <motion.div 
                          initial={{ height: 0, opacity: 0 }}
                          animate={{ height: 'auto', opacity: 1 }}
                          exit={{ height: 0, opacity: 0 }}
                          className="px-6 pb-6 pt-2 border-t border-outline-variant"
                        >
                          <div className="space-y-4 mt-2">
                            <p className="text-xs text-on-surface-variant leading-relaxed">
                              Đây là nội dung mở rộng của {panel.title}. Bạn có thể thêm các form, bảng hoặc thông tin chi tiết tại đây để tối ưu không gian hiển thị.
                            </p>
                            <div className="grid grid-cols-2 gap-4">
                              <div className="p-3 bg-surface-container rounded-lg border border-outline-variant">
                                <p className="text-[10px] font-bold text-on-surface-variant opacity-60 uppercase">Thông số A</p>
                                <p className="text-sm font-bold text-slate-700">1,284</p>
                              </div>
                              <div className="p-3 bg-slate-50 rounded-lg border border-slate-100">
                                <p className="text-[10px] font-bold text-slate-400 uppercase">Thông số B</p>
                                <p className="text-sm font-bold text-slate-700">85%</p>
                              </div>
                            </div>
                          </div>
                        </motion.div>
                      )}
                    </AnimatePresence>
                  </div>
                ))}
              </div>
            </section>

            {/* 7. WxDashboardSection + WxFeatureCard Section */}
            <section className="space-y-6">
              <div className="flex items-center space-x-2 text-primary font-bold text-xl">
                <Bolt className="w-6 h-6" />
                <span>WxDashboardSection + WxFeatureCard</span>
              </div>
              <div className="space-y-6">
                <div className="flex items-center space-x-3">
                  <h4 className="text-lg font-black text-slate-800 tracking-tight">Tính năng nổi bật</h4>
                  <div className="h-0.5 w-12 bg-primary/30 rounded-full" />
                </div>
                <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                  {[
                    { title: 'Quản lý TKQC', desc: 'Quản lý tài khoản quảng cáo Facebook một cách tập trung và hiệu quả.', icon: Layout },
                    { title: 'Kiểm tra BM', desc: 'Kiểm tra tình trạng Business Manager, limit chi tiêu và xác minh doanh nghiệp.', icon: Layers },
                    { title: 'Chia sẻ Pixel', desc: 'Chia sẻ pixel giữa các tài khoản quảng cáo và BM một cách an toàn.', icon: Share2 },
                  ].map((feat, i) => (
                    <div key={i} className="bg-white p-6 rounded-2xl border border-slate-100 shadow-sm hover:shadow-md transition-all group cursor-pointer">
                      <div className="w-12 h-12 rounded-xl bg-sky-50 flex items-center justify-center mb-4 group-hover:scale-110 transition-transform">
                        <feat.icon className="w-6 h-6 text-primary" />
                      </div>
                      <h5 className="text-sm font-bold text-slate-800 mb-2">{feat.title}</h5>
                      <p className="text-xs text-slate-500 leading-relaxed">{feat.desc}</p>
                    </div>
                  ))}
                </div>
              </div>
            </section>

            {/* 8. WxActionPopoverPanel Section */}
            <section className="space-y-6">
              <div className="flex items-center space-x-2 text-primary font-bold text-xl">
                <Menu className="w-6 h-6" />
                <span>WxActionPopoverPanel</span>
              </div>
              <div className="bg-slate-50 p-12 rounded-2xl border border-slate-200 border-dashed flex flex-col items-center justify-center space-y-4">
                <p className="text-xs text-slate-400 font-medium">Preview (inline):</p>
                <div className="relative">
                  <button 
                    onClick={() => setIsPopoverOpen(!isPopoverOpen)}
                    className="px-6 py-2.5 btn-primary rounded-xl text-sm font-bold shadow-lg shadow-primary/20 flex items-center space-x-2"
                  >
                    <Bolt className="w-4 h-4" />
                    <span>Mở Action Panel</span>
                    <ChevronDown className={`w-4 h-4 transition-transform ${isPopoverOpen ? 'rotate-180' : ''}`} />
                  </button>
                  
                  <AnimatePresence>
                    {isPopoverOpen && (
                      <motion.div 
                        initial={{ opacity: 0, y: 10, scale: 0.95 }}
                        animate={{ opacity: 1, y: 0, scale: 1 }}
                        exit={{ opacity: 0, y: 10, scale: 0.95 }}
                        className="absolute top-full mt-3 right-0 w-64 bg-white rounded-2xl shadow-2xl border border-slate-100 p-2 z-50"
                      >
                        <div className="space-y-1">
                          {[
                            { label: 'Chỉnh sửa thông tin', icon: FileText, color: 'text-slate-600' },
                            { label: 'Sao chép ID', icon: Bolt, color: 'text-slate-600' },
                            { label: 'Tải xuống báo cáo', icon: CloudUpload, color: 'text-slate-600' },
                            <div key="sep" className="h-px bg-slate-50 my-1 mx-2" />,
                            { label: 'Xóa bản ghi', icon: Trash2, color: 'text-rose-500' },
                          ].map((item, i) => (
                            typeof item === 'object' && 'label' in item ? (
                              <button key={i} className="w-full flex items-center space-x-3 px-3 py-2 rounded-xl hover:bg-slate-50 transition-colors text-left">
                                <item.icon className={`w-4 h-4 ${item.color}`} />
                                <span className={`text-xs font-bold ${item.color}`}>{item.label}</span>
                              </button>
                            ) : item
                          ))}
                        </div>
                      </motion.div>
                    )}
                  </AnimatePresence>
                </div>
              </div>
            </section>

            {/* 9. WxFilterBar Section */}
            <section className="space-y-6">
              <div className="flex items-center space-x-2 text-primary font-bold text-xl">
                <Search className="w-6 h-6" />
                <span>WxFilterBar</span>
              </div>
              <div className="bg-white p-4 rounded-xl border border-slate-100 shadow-sm flex flex-col md:flex-row md:items-center gap-4">
                <div className="relative flex-1">
                  <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-slate-400" />
                  <input 
                    type="text" 
                    placeholder="Tìm kiếm tài khoản, ID, người quản lý..." 
                    className="w-full pl-10 pr-4 py-2 bg-slate-50/50 border border-slate-100 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-primary/20 transition-all"
                  />
                </div>
                <div className="flex items-center gap-3">
                  <div className="relative">
                    <select className="appearance-none pl-3 pr-8 py-2 bg-white border border-slate-200 rounded-lg text-xs font-bold text-slate-600 focus:outline-none focus:ring-2 focus:ring-primary/20 transition-all cursor-pointer">
                      <option>Tất cả trạng thái</option>
                      <option>Active</option>
                      <option>Paused</option>
                      <option>Error</option>
                    </select>
                    <ChevronDown className="absolute right-2 top-1/2 -translate-y-1/2 w-3 h-3 text-slate-400 pointer-events-none" />
                  </div>
                  <div className="relative">
                    <select className="appearance-none pl-3 pr-8 py-2 bg-white border border-slate-200 rounded-lg text-xs font-bold text-slate-600 focus:outline-none focus:ring-2 focus:ring-primary/20 transition-all cursor-pointer">
                      <option>Sắp xếp theo</option>
                      <option>Mới nhất</option>
                      <option>Chi tiêu cao nhất</option>
                      <option>Tên A-Z</option>
                    </select>
                    <ChevronDown className="absolute right-2 top-1/2 -translate-y-1/2 w-3 h-3 text-slate-400 pointer-events-none" />
                  </div>
                  <button className="p-2 bg-slate-50 text-slate-500 rounded-lg hover:bg-slate-100 transition-colors">
                    <Calendar className="w-4 h-4" />
                  </button>
                  <button className="px-4 py-2 btn-primary rounded-lg text-xs font-bold shadow-lg shadow-primary/20">
                    Áp dụng
                  </button>
                </div>
              </div>
            </section>

            {/* 10. WxFormLayout Section */}
            <section className="space-y-6">
              <div className="flex items-center space-x-2 text-primary font-bold text-xl">
                <FileText className="w-6 h-6" />
                <span>WxFormLayout</span>
              </div>
              <div className="bg-white p-8 rounded-2xl border border-slate-100 shadow-sm space-y-8">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
                  <div className="space-y-4">
                    <h5 className="text-xs font-black text-slate-400 uppercase tracking-widest">THÔNG TIN CƠ BẢN</h5>
                    <div className="space-y-4">
                      <div className="space-y-1.5">
                        <label className="text-[10px] font-bold text-slate-500 uppercase">Tên chiến dịch</label>
                        <input type="text" placeholder="Nhập tên chiến dịch..." className="w-full px-4 py-2 border border-slate-100 rounded-lg text-sm focus:ring-2 focus:ring-primary/20" />
                      </div>
                      <div className="space-y-1.5">
                        <label className="text-[10px] font-bold text-slate-500 uppercase">Mục tiêu</label>
                        <select className="w-full px-4 py-2 border border-slate-100 rounded-lg text-sm focus:ring-2 focus:ring-primary/20">
                          <option>Tương tác bài viết</option>
                          <option>Chuyển đổi</option>
                          <option>Tin nhắn</option>
                        </select>
                      </div>
                    </div>
                  </div>
                  <div className="space-y-4">
                    <h5 className="text-xs font-black text-slate-400 uppercase tracking-widest">NGÂN SÁCH & LỊCH TRÌNH</h5>
                    <div className="space-y-4">
                      <div className="space-y-1.5">
                        <label className="text-[10px] font-bold text-slate-500 uppercase">Ngân sách hàng ngày</label>
                        <div className="relative">
                          <span className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400 text-sm">$</span>
                          <input type="number" defaultValue={100} className="w-full pl-8 pr-4 py-2 border border-slate-100 rounded-lg text-sm focus:ring-2 focus:ring-primary/20" />
                        </div>
                      </div>
                      <div className="grid grid-cols-2 gap-4">
                        <div className="space-y-1.5">
                          <label className="text-[10px] font-bold text-slate-500 uppercase">Ngày bắt đầu</label>
                          <input type="date" className="w-full px-4 py-2 border border-slate-100 rounded-lg text-sm focus:ring-2 focus:ring-primary/20" />
                        </div>
                        <div className="space-y-1.5">
                          <label className="text-[10px] font-bold text-slate-500 uppercase">Ngày kết thúc</label>
                          <input type="date" className="w-full px-4 py-2 border border-slate-100 rounded-lg text-sm focus:ring-2 focus:ring-primary/20" />
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
                <div className="h-px bg-slate-50" />
                <div className="flex items-center justify-end space-x-3">
                  <button className="px-6 py-2 text-slate-400 font-bold text-sm hover:text-slate-600">Hủy bỏ</button>
                  <button className="px-8 py-2 btn-primary rounded-lg text-sm font-bold shadow-lg shadow-primary/20">Lưu chiến dịch</button>
                </div>
              </div>
            </section>

            {/* 11. WxAuthPatterns Section */}
            <section className="space-y-6">
              <div className="flex items-center space-x-2 text-primary font-bold text-xl">
                <User className="w-6 h-6" />
                <span>WxAuthPatterns</span>
              </div>
              <div className="bg-slate-100 p-12 rounded-2xl border border-slate-200 border-dashed flex items-center justify-center">
                <div className="w-full max-w-sm bg-white p-8 rounded-3xl shadow-2xl border border-slate-100 space-y-8">
                  <div className="text-center space-y-2">
                    <div className="w-12 h-12 bg-primary rounded-2xl mx-auto flex items-center justify-center shadow-lg shadow-primary/20">
                      <Bolt className="w-7 h-7 text-white" />
                    </div>
                    <h3 className="text-xl font-black text-slate-800 tracking-tight">Chào mừng trở lại</h3>
                    <p className="text-xs text-slate-400 font-medium">Đăng nhập để quản lý hệ thống Wemake</p>
                  </div>
                  <div className="space-y-4">
                    <div className="space-y-1.5">
                      <label className="text-[10px] font-bold text-slate-500 uppercase ml-1">Email</label>
                      <input type="email" placeholder="admin@wemake.vn" className="w-full px-4 py-3 bg-slate-50 border border-slate-100 rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all" />
                    </div>
                    <div className="space-y-1.5">
                      <div className="flex justify-between items-center px-1">
                        <label className="text-[10px] font-bold text-slate-500 uppercase">Mật khẩu</label>
                        <button className="text-[10px] font-bold text-primary hover:underline">Quên mật khẩu?</button>
                      </div>
                      <input type="password" placeholder="••••••••" className="w-full px-4 py-3 bg-slate-50 border border-slate-100 rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all" />
                    </div>
                    <button className="w-full py-3 btn-gradient text-white rounded-xl text-sm font-black shadow-xl shadow-primary/30 transform active:scale-[0.98] transition-all">
                      ĐĂNG NHẬP NGAY
                    </button>
                  </div>
                  <div className="text-center">
                    <p className="text-[10px] text-slate-400 font-medium">Bạn chưa có tài khoản? <button className="text-primary font-bold hover:underline">Đăng ký miễn phí</button></p>
                  </div>
                </div>
              </div>
            </section>

            {/* 12. WxFeedbackPatterns Section */}
            <section className="space-y-6">
              <div className="flex items-center space-x-2 text-primary font-bold text-xl">
                <Activity className="w-6 h-6" />
                <span>WxFeedbackPatterns</span>
              </div>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
                <div className="bg-white p-8 rounded-2xl border border-slate-100 shadow-sm space-y-6">
                  <h5 className="text-xs font-black text-slate-400 uppercase tracking-widest">TOAST NOTIFICATIONS</h5>
                  <div className="space-y-3">
                    <div className="p-3 bg-slate-900 text-white rounded-xl shadow-2xl flex items-center justify-between">
                      <div className="flex items-center space-x-3">
                        <CheckCircle2 className="w-4 h-4 text-emerald-400" />
                        <span className="text-xs font-bold">Đã lưu thay đổi thành công!</span>
                      </div>
                      <button className="text-white/40 hover:text-white"><X className="w-4 h-4" /></button>
                    </div>
                    <div className="p-3 bg-white border border-slate-100 rounded-xl shadow-xl flex items-center justify-between">
                      <div className="flex items-center space-x-3">
                        <Loader2 className="w-4 h-4 text-primary animate-spin" />
                        <span className="text-xs font-bold text-slate-600">Đang đồng bộ dữ liệu...</span>
                      </div>
                      <span className="text-[10px] font-black text-primary">45%</span>
                    </div>
                  </div>
                </div>
                <div className="bg-white p-8 rounded-2xl border border-slate-100 shadow-sm space-y-6">
                  <h5 className="text-xs font-black text-slate-400 uppercase tracking-widest">PROGRESS & SKELETON</h5>
                  <div className="space-y-6">
                    <div className="space-y-2">
                      <div className="flex justify-between text-[10px] font-bold text-slate-500 uppercase">
                        <span>Dung lượng lưu trữ</span>
                        <span>8.5GB / 10GB</span>
                      </div>
                      <div className="h-2 bg-slate-100 rounded-full overflow-hidden">
                        <div className="h-full bg-primary w-[85%]" />
                      </div>
                    </div>
                    <div className="flex items-center space-x-4">
                      <div className="w-10 h-10 bg-slate-100 rounded-full animate-pulse" />
                      <div className="flex-1 space-y-2">
                        <div className="h-2 bg-slate-100 rounded-full w-1/2 animate-pulse" />
                        <div className="h-2 bg-slate-100 rounded-full w-full animate-pulse" />
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </section>

            {/* 13. WxDataVisualization Section */}
            <section className="space-y-6">
              <div className="flex items-center space-x-2 text-primary font-bold text-xl">
                <BarChart3 className="w-6 h-6" />
                <span>WxDataVisualization</span>
              </div>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
                <div className="bg-white p-8 rounded-2xl border border-slate-100 shadow-sm space-y-6">
                  <div className="flex items-center justify-between">
                    <h5 className="text-xs font-black text-slate-400 uppercase tracking-widest">BIỂU ĐỒ CHI TIÊU (7 NGÀY)</h5>
                    <TrendingUp className="w-4 h-4 text-emerald-500" />
                  </div>
                  <div className="h-64 w-full">
                    <ResponsiveContainer width="100%" height="100%">
                      <LineChart data={[
                        { name: 'T2', value: 400 },
                        { name: 'T3', value: 300 },
                        { name: 'T4', value: 600 },
                        { name: 'T5', value: 800 },
                        { name: 'T6', value: 500 },
                        { name: 'T7', value: 900 },
                        { name: 'CN', value: 1100 },
                      ]}>
                        <CartesianGrid strokeDasharray="3 3" vertical={false} stroke="#f1f5f9" />
                        <XAxis dataKey="name" axisLine={false} tickLine={false} tick={{ fontSize: 10, fill: '#94a3b8', fontWeight: 700 }} />
                        <YAxis axisLine={false} tickLine={false} tick={{ fontSize: 10, fill: '#94a3b8', fontWeight: 700 }} />
                        <Tooltip 
                          contentStyle={{ backgroundColor: '#fff', borderRadius: '12px', border: '1px solid #f1f5f9', boxShadow: '0 10px 15px -3px rgb(0 0 0 / 0.1)' }}
                          itemStyle={{ fontSize: '12px', fontWeight: 'bold', color: '#0088ff' }}
                        />
                        <Line type="monotone" dataKey="value" stroke="#0088ff" strokeWidth={3} dot={{ r: 4, fill: '#0088ff', strokeWidth: 2, stroke: '#fff' }} activeDot={{ r: 6 }} />
                      </LineChart>
                    </ResponsiveContainer>
                  </div>
                </div>
                <div className="bg-white p-8 rounded-2xl border border-slate-100 shadow-sm space-y-6">
                  <div className="flex items-center justify-between">
                    <h5 className="text-xs font-black text-slate-400 uppercase tracking-widest">PHÂN BỔ TÀI KHOẢN</h5>
                    <PieChartIcon className="w-4 h-4 text-primary" />
                  </div>
                  <div className="h-64 w-full flex items-center justify-center">
                    <ResponsiveContainer width="100%" height="100%">
                      <PieChart>
                        <Pie
                          data={[
                            { name: 'Active', value: 400 },
                            { name: 'Paused', value: 300 },
                            { name: 'Error', value: 100 },
                            { name: 'Completed', value: 200 },
                          ]}
                          cx="50%"
                          cy="50%"
                          innerRadius={60}
                          outerRadius={80}
                          paddingAngle={5}
                          dataKey="value"
                        >
                          {[
                            { name: 'Active', color: '#10b981' },
                            { name: 'Paused', color: '#f59e0b' },
                            { name: 'Error', color: '#f43f5e' },
                            { name: 'Completed', color: '#0088ff' },
                          ].map((entry, index) => (
                            <Cell key={`cell-${index}`} fill={entry.color} />
                          ))}
                        </Pie>
                        <Tooltip 
                          contentStyle={{ backgroundColor: '#fff', borderRadius: '12px', border: '1px solid #f1f5f9', boxShadow: '0 10px 15px -3px rgb(0 0 0 / 0.1)' }}
                          itemStyle={{ fontSize: '12px', fontWeight: 'bold' }}
                        />
                      </PieChart>
                    </ResponsiveContainer>
                  </div>
                </div>
              </div>
            </section>

            {/* 14. WxEmptyStatePatterns Section */}
            <section className="space-y-6">
              <div className="flex items-center space-x-2 text-primary font-bold text-xl">
                <FileText className="w-6 h-6" />
                <span>WxEmptyStatePatterns</span>
              </div>
              <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                <div className="bg-white p-8 rounded-2xl border border-slate-100 shadow-sm flex flex-col items-center justify-center text-center space-y-4">
                  <div className="w-16 h-16 bg-slate-50 rounded-full flex items-center justify-center">
                    <Search className="w-8 h-8 text-slate-200" />
                  </div>
                  <div>
                    <h6 className="text-sm font-bold text-slate-700">Không tìm thấy kết quả</h6>
                    <p className="text-[10px] text-slate-400 mt-1 leading-relaxed">Thử thay đổi từ khóa hoặc bộ lọc của bạn.</p>
                  </div>
                </div>
                <div className="bg-white p-8 rounded-2xl border border-slate-100 shadow-sm flex flex-col items-center justify-center text-center space-y-4">
                  <div className="w-16 h-16 bg-sky-50 rounded-full flex items-center justify-center">
                    <Plus className="w-8 h-8 text-primary" />
                  </div>
                  <div>
                    <h6 className="text-sm font-bold text-slate-700">Chưa có chiến dịch nào</h6>
                    <p className="text-[10px] text-slate-400 mt-1 leading-relaxed">Bắt đầu bằng cách tạo chiến dịch đầu tiên của bạn.</p>
                  </div>
                  <button className="px-4 py-1.5 btn-primary rounded-lg text-[10px] font-black uppercase">Tạo ngay</button>
                </div>
                <div className="bg-white p-8 rounded-2xl border border-slate-100 shadow-sm flex flex-col items-center justify-center text-center space-y-4">
                  <div className="w-16 h-16 bg-rose-50 rounded-full flex items-center justify-center">
                    <AlertCircle className="w-8 h-8 text-rose-400" />
                  </div>
                  <div>
                    <h6 className="text-sm font-bold text-slate-700">Lỗi kết nối</h6>
                    <p className="text-[10px] text-slate-400 mt-1 leading-relaxed">Vui lòng kiểm tra lại đường truyền internet.</p>
                  </div>
                  <button className="px-4 py-1.5 btn-secondary rounded-lg text-[10px] font-black uppercase">Thử lại</button>
                </div>
              </div>
            </section>

            {/* 15. WxRemovablePatterns Section */}
            <section className="space-y-6">
              <div className="flex items-center justify-between">
                <div className="flex items-center space-x-2 text-primary font-bold text-xl">
                  <Trash2 className="w-6 h-6" />
                  <span>WxRemovablePatterns</span>
                </div>
                {(removableFilters.length === 0 || removableUsers.length === 0) && (
                  <button 
                    onClick={() => {
                      setRemovableFilters(['Status: Active', 'Region: VN', 'Budget: >$500', 'Platform: FB']);
                      setRemovableUsers([
                        { id: 1, name: 'Nguyễn Văn A', role: 'Admin' },
                        { id: 2, name: 'Trần Thị B', role: 'Editor' }
                      ]);
                    }}
                    className="text-[10px] font-black text-primary uppercase tracking-widest flex items-center space-x-2 hover:bg-sky-50 px-3 py-1.5 rounded-lg transition-colors"
                  >
                    <RefreshCw className="w-3 h-3" />
                    <span>Khôi phục dữ liệu</span>
                  </button>
                )}
              </div>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
                <div className="bg-white p-8 rounded-2xl border border-slate-100 shadow-sm space-y-6">
                  <h5 className="text-xs font-black text-slate-400 uppercase tracking-widest">REMOVABLE FILTERS</h5>
                  <div className="flex flex-wrap gap-2">
                    {removableFilters.length > 0 ? (
                      removableFilters.map((filter, i) => (
                        <span key={i} className="px-3 py-1.5 bg-slate-50 text-slate-600 rounded-full text-[10px] font-bold flex items-center space-x-2 border border-slate-100">
                          <span>{filter}</span>
                          <button 
                            onClick={() => setRemovableFilters(prev => prev.filter((_, idx) => idx !== i))}
                            className="hover:text-rose-500 transition-colors"
                          >
                            <X className="w-3 h-3" />
                          </button>
                        </span>
                      ))
                    ) : (
                      <p className="text-[10px] text-slate-300 italic">Tất cả bộ lọc đã bị xóa</p>
                    )}
                  </div>
                </div>
                <div className="bg-white p-8 rounded-2xl border border-slate-100 shadow-sm space-y-6">
                  <h5 className="text-xs font-black text-slate-400 uppercase tracking-widest">REMOVABLE USERS</h5>
                  <div className="space-y-3">
                    {removableUsers.length > 0 ? (
                      removableUsers.map((user) => (
                        <div key={user.id} className="flex items-center justify-between p-3 bg-slate-50 rounded-xl border border-slate-100">
                          <div className="flex items-center space-x-3">
                            <div className="w-8 h-8 bg-primary/10 rounded-full flex items-center justify-center text-primary font-bold text-xs">
                              {user.name[0]}
                            </div>
                            <div>
                              <p className="text-xs font-bold text-slate-700">{user.name}</p>
                              <p className="text-[10px] text-slate-400">{user.role}</p>
                            </div>
                          </div>
                          <button 
                            onClick={() => setRemovableUsers(prev => prev.filter(u => u.id !== user.id))}
                            className="p-1.5 text-slate-300 hover:text-rose-500 hover:bg-rose-50 rounded-lg transition-all"
                          >
                            <Trash2 className="w-4 h-4" />
                          </button>
                        </div>
                      ))
                    ) : (
                      <p className="text-[10px] text-slate-300 italic">Tất cả người dùng đã bị xóa</p>
                    )}
                  </div>
                </div>
              </div>
            </section>

            {/* 16. WxNotificationPatterns Section */}
            <section className="space-y-6">
              <div className="flex items-center space-x-2 text-primary font-bold text-xl">
                <Bell className="w-6 h-6" />
                <span>WxNotificationPatterns</span>
              </div>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
                <div className="bg-white p-8 rounded-2xl border border-slate-100 shadow-sm space-y-6">
                  <h5 className="text-xs font-black text-slate-400 uppercase tracking-widest">SYSTEM NOTIFICATIONS</h5>
                  <div className="space-y-4">
                    {[
                      { title: 'Bảo trì hệ thống', desc: 'Hệ thống sẽ bảo trì vào lúc 02:00 sáng mai.', type: 'warning' },
                      { title: 'Cập nhật phiên bản', desc: 'Phiên bản v2.5.0 đã sẵn sàng để nâng cấp.', type: 'info' }
                    ].map((notif, i) => (
                      <div key={i} className="flex space-x-4 p-4 rounded-xl bg-slate-50 border border-slate-100">
                        <div className={`w-10 h-10 rounded-full flex items-center justify-center flex-shrink-0 ${notif.type === 'warning' ? 'bg-amber-100 text-amber-600' : 'bg-sky-100 text-primary'}`}>
                          {notif.type === 'warning' ? <AlertTriangle className="w-5 h-5" /> : <Info className="w-5 h-5" />}
                        </div>
                        <div className="space-y-1">
                          <p className="text-sm font-bold text-slate-800">{notif.title}</p>
                          <p className="text-[10px] text-slate-500 leading-relaxed">{notif.desc}</p>
                          <p className="text-[9px] font-bold text-slate-300 uppercase mt-2">2 giờ trước</p>
                        </div>
                      </div>
                    ))}
                  </div>
                </div>
                <div className="bg-white p-8 rounded-2xl border border-slate-100 shadow-sm space-y-6">
                  <h5 className="text-xs font-black text-slate-400 uppercase tracking-widest">USER ACTIVITY</h5>
                  <div className="space-y-4">
                    {[
                      { user: 'Admin', action: 'đã phê duyệt chiến dịch', target: 'Campaign_X', time: '5 phút trước' },
                      { user: 'Editor_1', action: 'đã thay đổi ngân sách', target: 'Campaign_Y', time: '15 phút trước' }
                    ].map((activity, i) => (
                      <div key={i} className="flex items-start space-x-3">
                        <div className="w-8 h-8 bg-slate-100 rounded-full flex items-center justify-center text-[10px] font-bold text-slate-500">
                          {activity.user[0]}
                        </div>
                        <div className="flex-1">
                          <p className="text-xs text-slate-600">
                            <span className="font-bold text-slate-800">{activity.user}</span> {activity.action} <span className="font-bold text-primary">{activity.target}</span>
                          </p>
                          <p className="text-[9px] text-slate-400 font-medium mt-0.5">{activity.time}</p>
                        </div>
                      </div>
                    ))}
                  </div>
                </div>
              </div>
            </section>

            {/* 17. WxCardPatterns Section */}
            <section className="space-y-6">
              <div className="flex items-center space-x-2 text-primary font-bold text-xl">
                <Layout className="w-6 h-6" />
                <span>WxCardPatterns</span>
              </div>
              <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                <div className="bg-white rounded-2xl border border-slate-100 shadow-sm overflow-hidden group">
                  <div className="h-32 bg-slate-100 relative overflow-hidden">
                    <div className="absolute inset-0 bg-gradient-to-br from-primary/20 to-transparent" />
                    <div className="absolute bottom-3 left-4 px-2 py-1 bg-white/80 backdrop-blur rounded text-[9px] font-black text-primary uppercase">Mới nhất</div>
                  </div>
                  <div className="p-6 space-y-3">
                    <h6 className="text-sm font-black text-slate-800 tracking-tight">Chiến dịch mùa hè 2026</h6>
                    <p className="text-[10px] text-slate-500 leading-relaxed">Tối ưu hóa chuyển đổi cho các sản phẩm thời trang nam.</p>
                    <div className="flex items-center justify-between pt-2">
                      <div className="flex -space-x-2">
                        {[1, 2, 3].map(i => (
                          <div key={i} className="w-6 h-6 rounded-full border-2 border-white bg-slate-200" />
                        ))}
                      </div>
                      <button className="text-primary hover:translate-x-1 transition-transform">
                        <ArrowRight className="w-4 h-4" />
                      </button>
                    </div>
                  </div>
                </div>
                <div className="bg-white p-6 rounded-2xl border border-slate-100 shadow-sm flex items-start space-x-4">
                  <div className="w-12 h-12 bg-emerald-50 rounded-xl flex items-center justify-center text-emerald-500">
                    <Coins className="w-6 h-6" />
                  </div>
                  <div className="flex-1 space-y-1">
                    <h6 className="text-sm font-black text-slate-800 tracking-tight">Ngân sách còn lại</h6>
                    <p className="text-xl font-black text-emerald-500">$12,450.00</p>
                    <div className="flex items-center space-x-1 text-[9px] font-bold text-emerald-600">
                      <TrendingUp className="w-3 h-3" />
                      <span>+12.5% so với tháng trước</span>
                    </div>
                  </div>
                </div>
                <div className="bg-slate-900 p-6 rounded-2xl shadow-2xl space-y-4 relative overflow-hidden">
                  <div className="absolute -right-4 -top-4 w-24 h-24 bg-primary/20 rounded-full blur-2xl" />
                  <div className="flex items-center justify-between">
                    <div className="w-10 h-10 bg-white/10 rounded-lg flex items-center justify-center text-white">
                      <Lock className="w-5 h-5" />
                    </div>
                    <span className="text-[9px] font-black text-white/40 uppercase tracking-widest">Security</span>
                  </div>
                  <div className="space-y-1">
                    <h6 className="text-sm font-black text-white tracking-tight">Xác thực 2 lớp</h6>
                    <p className="text-[10px] text-white/60 leading-relaxed">Tăng cường bảo mật cho tài khoản của bạn ngay hôm nay.</p>
                  </div>
                  <button className="w-full py-2 bg-white text-slate-900 rounded-lg text-[10px] font-black uppercase tracking-wider hover:bg-slate-100 transition-colors">Kích hoạt</button>
                </div>
              </div>
            </section>

            {/* 18. WxLayoutPatterns Section */}
            <section className="space-y-6">
              <div className="flex items-center space-x-2 text-primary font-bold text-xl">
                <Layers className="w-6 h-6" />
                <span>WxLayoutPatterns</span>
              </div>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
                <div className="bg-white rounded-2xl border border-slate-100 shadow-sm overflow-hidden flex flex-col h-80">
                  <div className="h-10 bg-slate-50 border-bottom border-slate-100 flex items-center px-4 space-x-2">
                    <div className="w-2 h-2 rounded-full bg-rose-400" />
                    <div className="w-2 h-2 rounded-full bg-amber-400" />
                    <div className="w-2 h-2 rounded-full bg-emerald-400" />
                  </div>
                  <div className="flex-1 flex">
                    <div className="w-16 bg-slate-900 flex flex-col items-center py-4 space-y-4">
                      <div className="w-8 h-8 bg-primary rounded-lg" />
                      <div className="w-8 h-8 bg-white/10 rounded-lg" />
                      <div className="w-8 h-8 bg-white/10 rounded-lg" />
                    </div>
                    <div className="flex-1 bg-slate-50 p-4 space-y-4">
                      <div className="h-8 bg-white rounded-lg shadow-sm" />
                      <div className="grid grid-cols-3 gap-4">
                        <div className="h-20 bg-white rounded-lg shadow-sm" />
                        <div className="h-20 bg-white rounded-lg shadow-sm" />
                        <div className="h-20 bg-white rounded-lg shadow-sm" />
                      </div>
                      <div className="h-24 bg-white rounded-lg shadow-sm" />
                    </div>
                  </div>
                </div>
                <div className="bg-white rounded-2xl border border-slate-100 shadow-sm overflow-hidden flex flex-col h-80">
                  <div className="h-10 bg-slate-50 border-bottom border-slate-100 flex items-center px-4 justify-between">
                    <div className="flex items-center space-x-2">
                      <div className="w-2 h-2 rounded-full bg-slate-300" />
                      <div className="w-16 h-2 bg-slate-200 rounded-full" />
                    </div>
                    <div className="w-6 h-6 bg-slate-200 rounded-full" />
                  </div>
                  <div className="flex-1 p-8 flex flex-col items-center justify-center space-y-6">
                    <div className="w-20 h-20 bg-slate-100 rounded-full" />
                    <div className="space-y-2 w-full max-w-[200px]">
                      <div className="h-3 bg-slate-200 rounded-full w-3/4 mx-auto" />
                      <div className="h-2 bg-slate-100 rounded-full w-1/2 mx-auto" />
                    </div>
                    <div className="grid grid-cols-2 gap-3 w-full max-w-[240px]">
                      <div className="h-8 bg-slate-50 rounded-lg border border-slate-100" />
                      <div className="h-8 bg-slate-50 rounded-lg border border-slate-100" />
                    </div>
                  </div>
                </div>
              </div>
            </section>
          </div>
        )}


        {activePage === 'advanced' && (
          <div className="space-y-12">
            {/* WxBreadcrumb Section */}
            <section className="space-y-4">
              <div className="flex items-center space-x-2 text-primary font-bold text-sm uppercase tracking-widest">
                <Layout className="w-4 h-4" />
                <span>WxBreadcrumb</span>
              </div>
              <div className="bg-surface-container-lowest p-4 rounded-xl border border-outline-variant shadow-sm transition-colors duration-300">
                <nav className="flex items-center space-x-2 text-xs font-medium">
                  <button 
                    className="flex items-center space-x-1 text-on-surface-variant opacity-60 hover:text-primary transition-colors"
                  >
                    <Home className="w-3.5 h-3.5" />
                    <span>Trang chủ</span>
                  </button>
                  <ChevronRight className="w-3 h-3 text-on-surface-variant opacity-30" />
                  <button 
                    className="text-on-surface-variant opacity-60 hover:text-primary transition-colors"
                  >
                    Quản lý TKQC
                  </button>
                  <ChevronRight className="w-3 h-3 text-on-surface-variant opacity-30" />
                  <span className="text-on-surface font-bold">Campaign_A</span>
                </nav>
              </div>
            </section>

            {/* WxStepper Section */}
            <section className="space-y-4">
              <div className="flex items-center space-x-2 text-primary font-bold text-sm uppercase tracking-widest">
                <List className="w-4 h-4" />
                <span>WxStepper</span>
              </div>
              <div className="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant space-y-10 relative overflow-hidden transition-colors duration-300">
                <AnimatePresence mode="wait">
                  {!isFinished ? (
                    <motion.div 
                      key="stepper-content"
                      initial={{ opacity: 0, y: 10 }}
                      animate={{ opacity: 1, y: 0 }}
                      exit={{ opacity: 0, scale: 0.95 }}
                      className="space-y-10"
                    >
                      <h3 className="text-[10px] font-bold text-on-surface-variant opacity-60 uppercase tracking-wider">Horizontal</h3>
                      <div className="flex items-center justify-between max-w-4xl mx-auto relative">
                        {/* Progress Line */}
                        <div className="absolute top-5 left-0 right-0 h-px bg-outline-variant -z-0" />
                        <motion.div 
                          className="absolute top-5 left-0 h-px btn-gradient -z-0" 
                          initial={{ width: 0 }}
                          animate={{ width: `${(activeStep / 3) * 100}%` }}
                          transition={{ duration: 0.5, ease: "easeInOut" }}
                        />
                        
                        {[
                          { icon: Home, title: 'Chọn BM', desc: 'Business Manager' },
                          { icon: Settings, title: 'Cấu hình', desc: 'Thiết lập proxy' },
                          { icon: Play, title: 'Chạy', desc: 'Khởi chạy task' },
                          { icon: FileText, title: 'Kết quả', desc: 'Xem báo cáo' },
                        ].map((step, i) => {
                          const isCompleted = i < activeStep;
                          const isActive = i === activeStep;
                          const isInactive = i > activeStep;
                          
                          return (
                            <div key={i} className="relative z-10 flex items-center space-x-4 bg-surface-container-lowest pr-4 transition-colors duration-300">
                              <motion.div 
                                className={`w-10 h-10 rounded-full flex items-center justify-center shadow-sm transition-all duration-300 ${
                                  isActive ? 'btn-gradient text-white ring-4 ring-primary/10' : 
                                  isCompleted ? 'bg-blue-600 text-white' : 
                                  'bg-surface-container text-on-surface-variant opacity-60 border border-outline-variant'
                                }`}
                                animate={{ scale: isActive ? 1.1 : 1 }}
                              >
                                {isCompleted ? (
                                  <motion.div
                                    initial={{ scale: 0 }}
                                    animate={{ scale: 1 }}
                                    transition={{ type: "spring", stiffness: 300, damping: 20 }}
                                  >
                                    <Check className="w-5 h-5" />
                                  </motion.div>
                                ) : (
                                  <step.icon className="w-5 h-5" />
                                )}
                              </motion.div>
                              <div className={`transition-all duration-300 ${isActive ? 'opacity-100' : isCompleted ? 'opacity-80' : 'opacity-40'}`}>
                                <p className={`text-sm font-bold leading-tight transition-colors ${isActive ? 'text-on-surface' : isCompleted ? 'text-blue-600' : 'text-on-surface-variant'}`}>
                                  {step.title}
                                </p>
                                <p className="text-[10px] text-on-surface-variant opacity-60 font-medium">{step.desc}</p>
                              </div>
                            </div>
                          );
                        })}
                      </div>
                      <div className="flex items-center justify-between pt-6 border-t border-outline-variant">
                        <button 
                          onClick={() => setActiveStep(Math.max(0, activeStep - 1))}
                          className={`text-sm font-bold transition-colors ${activeStep === 0 ? 'text-on-surface-variant opacity-20 cursor-not-allowed' : 'text-on-surface-variant hover:text-on-surface'}`}
                          disabled={activeStep === 0}
                        >
                          Quay lại
                        </button>
                        <button 
                          onClick={() => {
                            if (activeStep === 3) {
                              setIsFinished(true);
                            } else {
                              setActiveStep(activeStep + 1);
                            }
                          }}
                          className="px-8 py-2 btn-gradient text-white rounded-lg text-sm font-bold shadow-lg shadow-primary/20 transition-all active:scale-95"
                        >
                          {activeStep === 3 ? 'Hoàn thành' : 'Tiếp tục'}
                        </button>
                      </div>
                    </motion.div>
                  ) : (
                    <motion.div 
                      key="success-content"
                      initial={{ opacity: 0, scale: 0.9 }}
                      animate={{ opacity: 1, scale: 1 }}
                      className="flex flex-col items-center justify-center py-10 space-y-4"
                    >
                      <div className="w-20 h-20 rounded-full bg-primary/10 flex items-center justify-center">
                        <motion.div
                          initial={{ scale: 0 }}
                          animate={{ scale: 1 }}
                          transition={{ type: "spring", stiffness: 200, damping: 10, delay: 0.2 }}
                        >
                          <CheckCircle2 className="w-12 h-12 text-primary" />
                        </motion.div>
                      </div>
                      <div className="text-center space-y-2">
                        <h4 className="text-xl font-black text-on-surface">Hoàn tất quy trình!</h4>
                        <p className="text-sm text-on-surface-variant opacity-60 max-w-xs mx-auto">Tất cả các bước đã được thực hiện thành công. Bạn có thể bắt đầu quản lý task ngay bây giờ.</p>
                      </div>
                      <button 
                        onClick={() => {
                          setIsFinished(false);
                          setActiveStep(0);
                        }}
                        className="px-6 py-2 bg-surface-container text-on-surface-variant rounded-lg text-sm font-bold hover:bg-surface-container-high transition-colors"
                      >
                        Làm lại
                      </button>
                    </motion.div>
                  )}
                </AnimatePresence>
              </div>
            </section>

            {/* WxPageHeader Section */}
            <section className="space-y-4">
              <div className="flex items-center space-x-2 text-primary font-bold text-sm uppercase tracking-widest">
                <FileText className="w-4 h-4" />
                <span>WxPageHeader</span>
              </div>
              <div className="bg-surface-container-lowest p-6 rounded-xl border border-outline-variant shadow-sm space-y-4 transition-colors duration-300">
                <nav className="flex items-center space-x-2 text-[10px] font-bold text-on-surface-variant opacity-60 uppercase tracking-wider">
                  <span>Trang chủ</span>
                  <ChevronRight className="w-3 h-3" />
                  <span>Quản lý TKQC</span>
                  <ChevronRight className="w-3 h-3" />
                  <span className="text-primary">Campaign_A</span>
                </nav>
                  <div className="flex items-center justify-between">
                    <div className="flex items-center space-x-4">
                      <button 
                        onClick={() => setActivePage('patterns')}
                        className="p-2 hover:bg-surface-container rounded-full text-on-surface-variant transition-colors"
                      >
                        <ArrowLeft className="w-5 h-5" />
                      </button>
                      <div>
                        <h2 className="text-2xl font-black text-on-surface tracking-tight">Quản lý tài khoản</h2>
                        <p className="text-xs text-on-surface-variant font-medium">Quản lý tất cả tài khoản quảng cáo Facebook</p>
                      </div>
                    </div>
                    <div className="flex items-center space-x-3">
                      <button 
                        className="px-4 py-2 bg-surface-container-lowest border border-outline-variant rounded-lg text-sm font-bold text-on-surface-variant hover:bg-surface-container transition-colors flex items-center space-x-2"
                      >
                        <Share2 className="w-4 h-4" />
                        <span>Export</span>
                      </button>
                      <button 
                        className="px-5 py-2 btn-gradient text-white rounded-lg text-sm font-bold shadow-lg shadow-primary/20 flex items-center space-x-2"
                      >
                        <Plus className="w-4 h-4" />
                        <span>Thêm mới</span>
                      </button>
                    </div>
                  </div>
              </div>
            </section>

            {/* WxFileUpload Section */}
            <section className="space-y-4">
              <div className="flex items-center space-x-2 text-primary font-bold text-sm uppercase tracking-widest">
                <CloudUpload className="w-4 h-4" />
                <span>WxFileUpload</span>
              </div>
              <div className="bg-surface-container-lowest p-10 rounded-xl border border-outline-variant shadow-sm transition-colors duration-300">
                <div 
                  className="border-2 border-dashed border-outline-variant rounded-2xl p-12 flex flex-col items-center justify-center space-y-4 bg-surface-container/30 group hover:border-primary/50 transition-colors cursor-pointer"
                >
                  <div className="w-16 h-16 bg-primary/10 rounded-full flex items-center justify-center text-primary group-hover:scale-110 transition-transform">
                    <CloudUpload className="w-8 h-8" />
                  </div>
                  <div className="text-center">
                    <p className="text-sm font-bold text-on-surface">Kéo thả file hoặc <span className="text-primary">nhấn để chọn</span></p>
                    <p className="text-[10px] text-on-surface-variant font-medium mt-1">Chấp nhận: .csv, .xlsx, .txt • Tối đa: 5.0 MB</p>
                  </div>
                </div>
              </div>
            </section>

            {/* WxDatePicker & WxSlider Section */}
            <section className="space-y-4">
              <div className="flex items-center space-x-2 text-primary font-bold text-sm uppercase tracking-widest">
                <Calendar className="w-4 h-4" />
                <span>WxDatePicker & WxSlider</span>
              </div>
              <div className="bg-surface-container-lowest p-8 rounded-xl border border-outline-variant shadow-sm transition-colors duration-300">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-12">
                  <div className="space-y-2">
                    <label className="text-[10px] font-bold text-on-surface-variant opacity-60 uppercase tracking-wider">Ngày bắt đầu</label>
                    <div className="relative">
                      <input 
                        type="text" 
                        readOnly
                        placeholder="mm/dd/yyyy" 
                        className="w-full px-4 py-2 bg-surface-container border border-outline-variant rounded-lg text-sm text-on-surface focus:outline-none focus:ring-2 focus:ring-primary/20 cursor-pointer transition-colors" 
                      />
                      <Calendar className="absolute right-3 top-1/2 -translate-y-1/2 w-4 h-4 text-on-surface-variant opacity-40 pointer-events-none" />
                    </div>
                    <p className="text-[10px] text-on-surface-variant opacity-40 italic">Chọn ngày để lọc dữ liệu</p>
                  </div>
                  <div className="space-y-4">
                    <div className="flex justify-between items-center">
                      <label className="text-[10px] font-bold text-on-surface-variant opacity-60 uppercase tracking-wider">Ngân sách (%)</label>
                      <span className="text-xs font-bold text-primary">{sliderValue}</span>
                    </div>
                    <div className="relative h-6 flex items-center">
                      <div className="absolute w-full h-1.5 bg-surface-container rounded-full" />
                      <div className="absolute h-1.5 bg-primary rounded-full transition-all duration-300" style={{ width: `${sliderValue}%` }} />
                      <input 
                        type="range" 
                        min="0" 
                        max="100" 
                        step="5"
                        value={sliderValue}
                        onChange={(e) => setSliderValue(parseInt(e.target.value))}
                        className="absolute inset-0 w-full h-full opacity-0 cursor-pointer z-10" 
                      />
                      <div 
                        className="absolute w-4 h-4 bg-white border-2 border-primary rounded-full shadow-md transition-all duration-300 -translate-x-1/2" 
                        style={{ left: `${sliderValue}%` }} 
                      />
                    </div>
                    <div className="flex justify-between text-[10px] font-bold text-on-surface-variant opacity-20">
                      <span>0</span>
                      <span>100</span>
                    </div>
                    <p className="text-[10px] text-on-surface-variant opacity-60 font-medium">Phần trăm ngân sách phân bổ</p>
                  </div>
                </div>
              </div>
            </section>

            {/* WxDrawer Section */}
            <section className="space-y-4">
              <div className="flex items-center space-x-2 text-primary font-bold text-sm uppercase tracking-widest">
                <ExternalLink className="w-4 h-4" />
                <span>WxDrawer</span>
              </div>
              <div className="bg-surface-container-lowest p-8 rounded-xl border border-outline-variant shadow-sm transition-colors duration-300">
                <button 
                  onClick={() => setIsDrawerOpen(true)}
                  className="px-6 py-2.5 btn-gradient text-white rounded-lg text-sm font-bold shadow-lg shadow-primary/20 flex items-center space-x-2"
                >
                  <ArrowRight className="w-4 h-4" />
                  <span>Mở Drawer</span>
                </button>
              </div>
            </section>

            {/* WxTimeline Section */}
            <section className="space-y-4">
              <div className="flex items-center space-x-2 text-primary font-bold text-sm uppercase tracking-widest">
                <List className="w-4 h-4" />
                <span>WxTimeline</span>
              </div>
              <div className="bg-surface-container-lowest p-8 rounded-xl border border-outline-variant shadow-sm transition-colors duration-300">
                <div className="space-y-0">
                  {[
                    { title: 'Khởi tạo chiến dịch', time: '08:30', desc: 'Tạo Campaign_A với ngân sách $5,000', icon: Plus, color: 'bg-blue-500' },
                    { title: 'Phê duyệt quảng cáo', time: '10:15', desc: 'Facebook đã duyệt 3/3 ads', icon: Check, color: 'bg-emerald-500' },
                    { title: 'Phát hiện lỗi proxy', time: '14:00', desc: 'Proxy #12 timeout, đã tự động chuyển sang proxy backup', icon: AlertTriangle, color: 'bg-orange-500' },
                    { title: 'Hoàn thành import', time: '16:45', desc: 'Import 1,234 tài khoản vào hệ thống', icon: Download, color: 'bg-sky-500' },
                  ].map((item, i, arr) => (
                    <div key={i} className="flex gap-6">
                      <div className="flex flex-col items-center">
                        <div className={`w-8 h-8 rounded-full ${item.color} text-white flex items-center justify-center shadow-sm z-10`}>
                          <item.icon className="w-4 h-4" />
                        </div>
                        {i !== arr.length - 1 && <div className="w-0.5 h-16 bg-surface-container" />}
                      </div>
                      <div className="pb-8">
                        <div className="flex items-center space-x-2">
                          <h4 className="text-sm font-bold text-on-surface">{item.title}</h4>
                          <span className="text-[10px] font-bold text-on-surface-variant opacity-40">{item.time}</span>
                        </div>
                        <p className="text-xs text-on-surface-variant mt-1 font-medium">{item.desc}</p>
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            </section>

            {/* WxTreeView Section */}
            <section className="space-y-4">
              <div className="flex items-center space-x-2 text-primary font-bold text-sm uppercase tracking-widest">
                <Folder className="w-4 h-4" />
                <span>WxTreeView</span>
              </div>
              <div className="bg-surface-container-lowest p-6 rounded-xl border border-outline-variant shadow-sm max-w-sm transition-colors duration-300">
                <div className="space-y-1">
                  <div 
                    onClick={() => toggleNode('bm001')}
                    className="flex items-center space-x-2 p-2 hover:bg-surface-container rounded-lg cursor-pointer group transition-colors"
                  >
                    <ChevronDown className={`w-4 h-4 text-on-surface-variant opacity-40 transition-transform ${expandedNodes.includes('bm001') ? '' : '-rotate-90'}`} />
                    <Briefcase className="w-4 h-4 text-on-surface-variant opacity-60" />
                    <span className="text-sm font-bold text-on-surface">Business Manager 001</span>
                  </div>
                  {expandedNodes.includes('bm001') && (
                    <div className="pl-6 space-y-1">
                      <div 
                        className="flex items-center space-x-2 p-2 hover:bg-surface-container rounded-lg cursor-pointer transition-colors"
                      >
                        <User className="w-4 h-4 text-on-surface-variant opacity-40" />
                        <span className="text-sm text-on-surface-variant font-medium">Tài khoản QC #101</span>
                      </div>
                      <div 
                        className="flex items-center space-x-2 p-2 hover:bg-surface-container rounded-lg cursor-pointer transition-colors"
                      >
                        <User className="w-4 h-4 text-on-surface-variant opacity-40" />
                        <span className="text-sm text-on-surface-variant font-medium">Tài khoản QC #102</span>
                      </div>
                      <div 
                        onClick={() => toggleNode('pages')}
                        className="flex items-center space-x-2 p-2 hover:bg-surface-container rounded-lg cursor-pointer transition-colors"
                      >
                        <ChevronRight className={`w-4 h-4 text-on-surface-variant opacity-20 transition-transform ${expandedNodes.includes('pages') ? 'rotate-90' : ''}`} />
                        <Folder className="w-4 h-4 text-on-surface-variant opacity-40" />
                        <span className="text-sm text-on-surface-variant font-medium">Pages</span>
                      </div>
                      {expandedNodes.includes('pages') && (
                        <div className="pl-6 py-1 text-xs text-on-surface-variant opacity-40 font-medium">
                          No pages found
                        </div>
                      )}
                    </div>
                  )}
                  <div 
                    onClick={() => toggleNode('bm002')}
                    className="flex items-center space-x-2 p-2 hover:bg-surface-container rounded-lg cursor-pointer transition-colors"
                  >
                    <ChevronRight className={`w-4 h-4 text-on-surface-variant opacity-20 transition-transform ${expandedNodes.includes('bm002') ? 'rotate-90' : ''}`} />
                    <Briefcase className="w-4 h-4 text-on-surface-variant opacity-60" />
                    <span className="text-sm font-bold text-on-surface">Business Manager 002</span>
                  </div>
                </div>
              </div>
            </section>

            {/* WxToast (Pure) Section */}
            <section className="space-y-4">
              <div className="flex items-center space-x-2 text-primary font-bold text-sm uppercase tracking-widest">
                <Bell className="w-4 h-4" />
                <span>WxToast (Pure)</span>
              </div>
              <div className="bg-surface-container-lowest p-8 rounded-xl border border-outline-variant shadow-sm transition-colors duration-300">
                <div className="flex flex-wrap gap-4">
                  <button 
                    onClick={() => addToast('info', 'Thông tin', 'Hệ thống sẽ bảo trì lúc 2:00 AM.')}
                    className="px-6 py-2.5 bg-gradient-to-r from-[#00c6ff] to-[#0072ff] text-white rounded-xl text-sm font-bold shadow-lg shadow-blue-500/20 flex items-center space-x-2 hover:scale-105 transition-all active:scale-95"
                  >
                    <Info className="w-4 h-4" />
                    <span>Info Toast</span>
                  </button>
                  <button 
                    onClick={() => addToast('success', 'Thành công', 'Nhập hoàn tất — 1.234 tài khoản.')}
                    className="px-6 py-2.5 bg-[#10b981] text-white rounded-xl text-sm font-bold shadow-lg shadow-emerald-500/20 flex items-center space-x-2 hover:scale-105 transition-all active:scale-95"
                  >
                    <CheckCircle2 className="w-4 h-4" />
                    <span>Success Toast</span>
                  </button>
                  <button 
                    onClick={() => addToast('warning', 'Cảnh báo', 'Proxy sắp hết hạn trong 3 ngày.')}
                    className="px-6 py-2.5 bg-surface-container-lowest border border-orange-200/30 text-orange-500 rounded-xl text-sm font-bold hover:bg-orange-500/10 transition-all flex items-center space-x-2 hover:scale-105 active:scale-95 shadow-sm"
                  >
                    <AlertTriangle className="w-4 h-4" />
                    <span>Warning Toast</span>
                  </button>
                  <button 
                    onClick={() => addToast('error', 'Lỗi', 'Không thể kết nối API Facebook.')}
                    className="px-6 py-2.5 bg-[#ef4444] text-white rounded-xl text-sm font-bold shadow-lg shadow-red-500/20 flex items-center space-x-2 hover:scale-105 transition-transform active:scale-95"
                  >
                    <X className="w-4 h-4" />
                    <span>Error Toast</span>
                  </button>
                </div>
              </div>
            </section>

            {/* WxSideNav (Preview) Section */}
            <section className="space-y-4">
              <div className="flex items-center space-x-2 text-primary font-bold text-sm uppercase tracking-widest">
                <List className="w-4 h-4" />
                <span>WxSideNav (Preview)</span>
              </div>
              <div className="bg-surface-container-lowest p-4 rounded-xl border border-outline-variant shadow-sm max-w-[240px] transition-colors duration-300">
                <div className="space-y-1">
                  <button 
                    onClick={() => setActiveSideNav('dashboard')}
                    className={`w-full flex items-center space-x-3 px-3 py-2 rounded-lg transition-colors ${activeSideNav === 'dashboard' ? 'bg-primary/10 text-primary' : 'text-on-surface-variant hover:bg-surface-container'}`}
                  >
                    <Layout className="w-4 h-4" />
                    <span className="text-sm font-bold">Dashboard</span>
                  </button>
                  <button 
                    onClick={() => setActiveSideNav('accounts')}
                    className={`w-full flex items-center justify-between px-3 py-2 rounded-lg transition-colors ${activeSideNav === 'accounts' ? 'bg-primary/10 text-primary' : 'text-on-surface-variant hover:bg-surface-container'}`}
                  >
                    <div className="flex items-center space-x-3">
                      <Briefcase className="w-4 h-4" />
                      <span className="text-sm font-bold">Tài khoản QC</span>
                    </div>
                    <span className="bg-primary/20 text-primary text-[10px] px-1.5 py-0.5 rounded-full font-bold">12</span>
                  </button>
                  <button 
                    onClick={() => setActiveSideNav('campaigns')}
                    className={`w-full flex items-center space-x-3 px-3 py-2 rounded-lg transition-colors ${activeSideNav === 'campaigns' ? 'bg-primary/10 text-primary' : 'text-on-surface-variant hover:bg-surface-container'}`}
                  >
                    <Bolt className="w-4 h-4" />
                    <span className="text-sm font-bold">Chiến dịch</span>
                  </button>
                  <div className="h-px bg-outline-variant my-2 mx-2" />
                  <p className="px-3 text-[10px] font-bold text-on-surface-variant opacity-40 uppercase tracking-widest mb-1">Hệ thống</p>
                  <button 
                    onClick={() => setActiveSideNav('settings')}
                    className={`w-full flex items-center space-x-3 px-3 py-2 rounded-lg transition-colors ${activeSideNav === 'settings' ? 'bg-primary/10 text-primary' : 'text-on-surface-variant hover:bg-surface-container'}`}
                  >
                    <Settings className="w-4 h-4" />
                    <span className="text-sm font-bold">Cài đặt</span>
                  </button>
                  <button 
                    onClick={() => setActiveSideNav('logs')}
                    className={`w-full flex items-center space-x-3 px-3 py-2 rounded-lg transition-colors ${activeSideNav === 'logs' ? 'bg-primary/10 text-primary' : 'text-on-surface-variant hover:bg-surface-container'}`}
                  >
                    <List className="w-4 h-4" />
                    <span className="text-sm font-bold">Nhật ký</span>
                  </button>
                </div>
              </div>
            </section>
          </div>
        )}

        {activePage === 'dialogs' && (
          <div className="space-y-12">
            {/* Dialog Components Section */}
            <section className="space-y-6">
              <div className="flex items-center space-x-2">
                <ExternalLink className="w-6 h-6 text-primary" />
                <h2 className="text-2xl font-bold text-on-surface tracking-tight">Dialog Components</h2>
              </div>
              <div className="bg-surface-container-lowest rounded-2xl p-8 shadow-sm border border-outline-variant transition-colors duration-300">
                <p className="text-on-surface-variant opacity-60 text-sm mb-6 font-medium">Bấm nút để xem dialog:</p>
                <div className="flex flex-wrap gap-4 mb-4">
                  <button 
                    onClick={() => setModalConfig({ isOpen: true, type: 'info', title: 'Thông báo', message: 'Đây là thông báo thông tin.' })}
                    className="px-6 py-2.5 bg-gradient-to-r from-[#00c6ff] to-[#0072ff] text-white rounded-xl text-sm font-bold shadow-lg shadow-blue-500/20 flex items-center space-x-2 hover:scale-105 transition-all active:scale-95"
                  >
                    <Info className="w-4 h-4" />
                    <span>Message (Info)</span>
                  </button>
                  <button 
                    onClick={() => setModalConfig({ isOpen: true, type: 'success', title: 'Thành công', message: 'Thao tác đã được thực hiện thành công.' })}
                    className="px-6 py-2.5 bg-[#10b981] text-white rounded-xl text-sm font-bold shadow-lg shadow-emerald-500/20 flex items-center space-x-2 hover:scale-105 transition-all active:scale-95"
                  >
                    <CheckCircle2 className="w-4 h-4" />
                    <span>Message (Success)</span>
                  </button>
                  <button 
                    onClick={() => setModalConfig({ isOpen: true, type: 'error', title: 'Lỗi', message: 'Đã có lỗi xảy ra. Vui lòng thử lại.' })}
                    className="px-6 py-2.5 bg-[#ef4444] text-white rounded-xl text-sm font-bold shadow-lg shadow-red-500/20 flex items-center space-x-2 hover:scale-105 transition-all active:scale-95"
                  >
                    <X className="w-4 h-4" />
                    <span>Message (Error)</span>
                  </button>
                  <button 
                    onClick={() => setModalConfig({ isOpen: true, type: 'question', title: 'Xác nhận', message: 'Bạn có chắc chắn muốn thực hiện hành động này?' })}
                    className="px-6 py-2.5 bg-[#06b6d4] text-white rounded-xl text-sm font-bold shadow-lg shadow-cyan-500/20 flex items-center space-x-2 hover:scale-105 transition-all active:scale-95"
                  >
                    <HelpCircle className="w-4 h-4" />
                    <span>Message (Question)</span>
                  </button>
                  <button 
                    onClick={() => setModalConfig({ isOpen: true, type: 'result', title: 'Kết quả kiểm tra', message: '3 Business Managers' })}
                    className="px-6 py-2.5 bg-white border border-slate-200 text-slate-600 rounded-xl text-sm font-bold hover:bg-slate-50 transition-all flex items-center space-x-2 shadow-sm active:scale-95"
                  >
                    <List className="w-4 h-4" />
                    <span>Result Dialog</span>
                  </button>
                </div>
                <div className="flex flex-wrap gap-4">
                  <button 
                    onClick={() => setModalConfig({ isOpen: true, type: 'config', title: 'Cấu hình chạy', message: 'Proxy và đồng thời' })}
                    className="px-6 py-2.5 bg-white border border-slate-200 text-slate-600 rounded-xl text-sm font-bold hover:bg-slate-50 transition-all flex items-center space-x-2 shadow-sm active:scale-95"
                  >
                    <Settings className="w-4 h-4" />
                    <span>Config Sidebar Dialog</span>
                  </button>
                </div>
              </div>
            </section>

            {/* WxModal Section */}
            <section className="space-y-6">
              <div className="flex items-center space-x-2">
                <Square className="w-6 h-6 text-primary" />
                <h2 className="text-2xl font-bold text-on-surface tracking-tight">WxModal & WxConfirmModal (Pure)</h2>
              </div>
              <div className="bg-surface-container-lowest rounded-2xl p-8 shadow-sm border border-outline-variant transition-colors duration-300">
                <p className="text-on-surface-variant opacity-60 text-sm mb-6 font-medium">Modal thuần không cần PrimeVue:</p>
                <div className="flex flex-wrap items-center gap-4">
                  <button 
                    onClick={() => setModalConfig({ isOpen: true, type: 'campaign-form', title: 'Tạo chiến dịch mới', message: 'Điền thông tin chiến dịch' })}
                    className="px-6 py-2.5 bg-[#06b6d4] text-white rounded-xl text-sm font-bold shadow-lg shadow-cyan-500/20 flex items-center space-x-2 hover:scale-105 transition-all active:scale-95"
                  >
                    <ExternalLink className="w-4 h-4" />
                    <span>Modal Default</span>
                  </button>
                  <button 
                    onClick={() => setModalConfig({ isOpen: true, type: 'system-info', title: 'Cập nhật hệ thống', message: 'Hệ thống sẽ được nâng cấp lên phiên bản v2.5.0 vào ngày 25/03/2026.' })}
                    className="px-6 py-2.5 bg-gradient-to-r from-[#00c6ff] to-[#0072ff] text-white rounded-xl text-sm font-bold shadow-lg shadow-blue-500/20 flex items-center space-x-2 hover:scale-105 transition-all active:scale-95"
                  >
                    <Info className="w-4 h-4" />
                    <span>Modal Info</span>
                  </button>
                  <button 
                    onClick={() => setModalConfig({ isOpen: true, type: 'delete', title: 'Xác nhận xóa', message: 'Bạn có chắc chắn muốn xóa mục này? Hành động này không thể hoàn tác.' })}
                    className="px-6 py-2.5 bg-[#ef4444] text-white rounded-xl text-sm font-bold shadow-lg shadow-red-500/20 flex items-center space-x-2 hover:scale-105 transition-all active:scale-95"
                  >
                    <Trash2 className="w-4 h-4" />
                    <span>Confirm Delete</span>
                  </button>
                  <button 
                    onClick={() => setModalConfig({ isOpen: true, type: 'warning', title: 'Cảnh báo', message: 'Hành động này có thể ảnh hưởng đến hệ thống.' })}
                    className="px-6 py-2.5 bg-white border border-slate-200 text-slate-600 rounded-xl text-sm font-bold hover:bg-slate-50 transition-all flex items-center space-x-2 shadow-sm active:scale-95"
                  >
                    <AlertTriangle className="w-4 h-4 text-orange-500" />
                    <span>Confirm Warning</span>
                  </button>
                </div>
              </div>
            </section>
          </div>
        )}

        {activePage === 'tokens' && (
          <div className="flex flex-col lg:flex-row gap-12">
            {/* Side Navigation for Tokens */}
            <aside className="w-full lg:w-64 flex-shrink-0 sticky top-32 h-fit space-y-2">
              <div className="mb-8 px-4">
                <h3 className="text-primary font-bold text-lg">System Tokens</h3>
                <p className="text-[10px] text-on-surface-variant opacity-60 font-medium">v2.4.0-stable</p>
              </div>
              <a 
                href="#brand-colors" 
                onClick={() => setActiveTokenSection('brand-colors')}
                className={`flex items-center gap-3 px-4 py-2.5 rounded-xl transition-all ${
                  activeTokenSection === 'brand-colors' 
                    ? 'bg-primary text-white shadow-lg shadow-primary/20' 
                    : 'text-on-surface-variant hover:bg-surface-container'
                }`}
              >
                <Palette className={`w-4 h-4 ${activeTokenSection === 'brand-colors' ? 'text-white' : ''}`} />
                <span className="text-sm font-bold">Brand Colors</span>
              </a>
              <a 
                href="#gradients" 
                onClick={() => setActiveTokenSection('gradients')}
                className={`flex items-center gap-3 px-4 py-2.5 rounded-xl transition-all ${
                  activeTokenSection === 'gradients' 
                    ? 'bg-primary text-white shadow-lg shadow-primary/20' 
                    : 'text-on-surface-variant hover:bg-surface-container'
                }`}
              >
                <div className={`w-4 h-4 rounded bg-gradient-to-br from-cyan-400 to-blue-600 ${activeTokenSection === 'gradients' ? 'ring-2 ring-white/50' : ''}`} />
                <span className="text-sm font-bold">Gradients</span>
              </a>
              <a 
                href="#typography" 
                onClick={() => setActiveTokenSection('typography')}
                className={`flex items-center gap-3 px-4 py-2.5 rounded-xl transition-all ${
                  activeTokenSection === 'typography' 
                    ? 'bg-primary text-white shadow-lg shadow-primary/20' 
                    : 'text-on-surface-variant hover:bg-surface-container'
                }`}
              >
                <Type className={`w-4 h-4 ${activeTokenSection === 'typography' ? 'text-white' : ''}`} />
                <span className="text-sm font-bold">Typography</span>
              </a>
              <a 
                href="#foundation-surfaces" 
                onClick={() => setActiveTokenSection('foundation-surfaces')}
                className={`flex items-center gap-3 px-4 py-2.5 rounded-xl transition-all ${
                  activeTokenSection === 'foundation-surfaces' 
                    ? 'bg-primary text-white shadow-lg shadow-primary/20' 
                    : 'text-on-surface-variant hover:bg-surface-container'
                }`}
              >
                <Layers className={`w-4 h-4 ${activeTokenSection === 'foundation-surfaces' ? 'text-white' : ''}`} />
                <span className="text-sm font-bold">Foundation Surfaces</span>
              </a>
              <a 
                href="#density-modes" 
                onClick={() => setActiveTokenSection('density-modes')}
                className={`flex items-center gap-3 px-4 py-2.5 rounded-xl transition-all ${
                  activeTokenSection === 'density-modes' 
                    ? 'bg-primary text-white shadow-lg shadow-primary/20' 
                    : 'text-on-surface-variant hover:bg-surface-container'
                }`}
              >
                <Menu className={`w-4 h-4 ${activeTokenSection === 'density-modes' ? 'text-white' : ''}`} />
                <span className="text-sm font-bold">Density Modes</span>
              </a>
            </aside>

            {/* Main Content for Tokens */}
            <div className="flex-1 space-y-12">
              {/* Brand Colors */}
              <section id="brand-colors" className="bg-surface-container-lowest rounded-2xl p-10 shadow-sm border border-outline-variant space-y-8 transition-colors duration-300">
                <div className="flex items-center gap-3">
                  <Palette className="w-5 h-5 text-primary" />
                  <h2 className="text-xl font-black text-on-surface tracking-tight">Brand Colors</h2>
                </div>
                
                <div className="space-y-8">
                  <div className="space-y-4">
                    <p className="text-[10px] font-bold text-on-surface-variant opacity-60 uppercase tracking-widest">Cyan spectrum</p>
                    <div className="flex flex-wrap gap-3">
                      {['#E0F2FE', '#BAE6FD', '#7DD3FC', '#38BDF8', '#0EA5E9', '#0284C7', '#0369A1', '#075985', '#0C4A6E', '#082F49'].map((color, i) => (
                        <div key={i} className="group relative">
                          <div 
                            className="w-12 h-12 rounded-lg shadow-sm cursor-pointer transition-transform hover:scale-110 active:scale-95" 
                            style={{ backgroundColor: color }}
                            onClick={() => copyToClipboard(color)}
                          />
                          <div className="absolute top-full left-1/2 -translate-x-1/2 mt-2 px-2 py-1 bg-surface-container-high text-on-surface text-[10px] rounded opacity-0 group-hover:opacity-100 transition-opacity whitespace-nowrap z-10 border border-outline-variant">
                            {color}
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>

                  <div className="space-y-4">
                    <p className="text-[10px] font-bold text-on-surface-variant opacity-60 uppercase tracking-widest">Blue spectrum</p>
                    <div className="flex flex-wrap gap-3">
                      {['#EFF6FF', '#DBEAFE', '#BFDBFE', '#93C5FD', '#60A5FA', '#3B82F6', '#2563EB', '#1D4ED8', '#1E40AF', '#1E3A8A'].map((color, i) => (
                        <div key={i} className="group relative">
                          <div 
                            className="w-12 h-12 rounded-lg shadow-sm cursor-pointer transition-transform hover:scale-110 active:scale-95" 
                            style={{ backgroundColor: color }}
                            onClick={() => copyToClipboard(color)}
                          />
                          <div className="absolute top-full left-1/2 -translate-x-1/2 mt-2 px-2 py-1 bg-surface-container-high text-on-surface text-[10px] rounded opacity-0 group-hover:opacity-100 transition-opacity whitespace-nowrap z-10 border border-outline-variant">
                            {color}
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>
                </div>
              </section>

              {/* Gradients */}
              <section id="gradients" className="bg-surface-container-lowest rounded-2xl p-10 shadow-sm border border-outline-variant space-y-8 transition-colors duration-300">
                <div className="flex items-center gap-3">
                  <div className="w-5 h-5 rounded bg-gradient-to-br from-cyan-400 to-blue-600" />
                  <h2 className="text-xl font-black text-on-surface tracking-tight">Gradients</h2>
                </div>
                
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                  <div className="h-24 rounded-xl flex items-center justify-center border border-sky-100 bg-gradient-to-r from-sky-200 to-sky-300">
                    <span className="text-sm font-bold text-sky-900">Page Background</span>
                  </div>
                  <div className="h-24 rounded-xl flex items-center justify-center shadow-lg shadow-blue-500/20 bg-gradient-to-r from-cyan-400 to-blue-600">
                    <span className="text-sm font-bold text-white">Header / Dialog</span>
                  </div>
                  <div className="h-24 rounded-xl flex items-center justify-center shadow-lg shadow-blue-600/20 bg-gradient-to-r from-blue-500 to-blue-700">
                    <span className="text-sm font-bold text-white">CTA Button</span>
                  </div>
                  <div className="h-24 rounded-xl flex items-center justify-center shadow-lg shadow-emerald-500/20 bg-gradient-to-r from-teal-400 to-emerald-500">
                    <span className="text-sm font-bold text-white">Success</span>
                  </div>
                  <div className="h-24 rounded-xl flex items-center justify-center shadow-lg shadow-red-500/20 bg-gradient-to-r from-rose-500 to-red-600">
                    <span className="text-sm font-bold text-white">Danger</span>
                  </div>
                  <div className="h-24 rounded-xl flex items-center justify-center shadow-lg shadow-orange-500/20 bg-gradient-to-r from-orange-400 to-amber-500">
                    <span className="text-sm font-bold text-white">Warning</span>
                  </div>
                </div>
              </section>

              {/* Typography */}
              <section id="typography" className="bg-surface-container-lowest rounded-2xl p-10 shadow-sm border border-outline-variant space-y-10 transition-colors duration-300">
                <div className="flex items-center gap-3 pb-6 border-b border-outline-variant">
                  <Type className="w-5 h-5 text-primary" />
                  <h2 className="text-xl font-black text-on-surface tracking-tight">Typography</h2>
                </div>
                
                <div className="space-y-8">
                  <div className="grid grid-cols-1 md:grid-cols-[240px_1fr] gap-6 items-center">
                    <div className="space-y-1">
                      <p className="text-[10px] font-bold text-primary uppercase tracking-widest">wx-heading-1</p>
                      <p className="text-[10px] text-on-surface-variant opacity-60 font-medium">Page Title (20px/800)</p>
                    </div>
                    <h1 className="text-[20px] font-black text-on-surface tracking-tight">Page Title</h1>
                  </div>

                  <div className="grid grid-cols-1 md:grid-cols-[240px_1fr] gap-6 items-center">
                    <div className="space-y-1">
                      <p className="text-[10px] font-bold text-primary uppercase tracking-widest">wx-heading-2</p>
                      <p className="text-[10px] text-on-surface-variant opacity-60 font-medium">Section Title (18px/700)</p>
                    </div>
                    <h2 className="text-[18px] font-bold text-on-surface">Section Title</h2>
                  </div>

                  <div className="grid grid-cols-1 md:grid-cols-[240px_1fr] gap-6 items-center">
                    <div className="space-y-1">
                      <p className="text-[10px] font-bold text-primary uppercase tracking-widest">wx-heading-3</p>
                      <p className="text-[10px] text-on-surface-variant opacity-60 font-medium">Subsection (15px/700)</p>
                    </div>
                    <h3 className="text-[15px] font-bold text-on-surface">Subsection</h3>
                  </div>

                  <div className="grid grid-cols-1 md:grid-cols-[240px_1fr] gap-6 items-center">
                    <div className="space-y-1">
                      <p className="text-[10px] font-bold text-primary uppercase tracking-widest">wx-body</p>
                      <p className="text-[10px] text-on-surface-variant opacity-60 font-medium">Body text (14px/400)</p>
                    </div>
                    <p className="text-sm text-on-surface-variant leading-relaxed">Body text - The quick brown fox jumps over the lazy dog.</p>
                  </div>

                  <div className="grid grid-cols-1 md:grid-cols-[240px_1fr] gap-6 items-center pt-6 border-t border-outline-variant">
                    <div className="space-y-1">
                      <p className="text-[10px] font-bold text-primary uppercase tracking-widest">wx-gradient-text</p>
                      <p className="text-[10px] text-on-surface-variant opacity-60 font-medium">Gradient Text Showcase</p>
                    </div>
                    <span className="text-2xl font-black uppercase bg-gradient-to-r from-cyan-400 to-blue-600 bg-clip-text text-transparent tracking-tighter">
                      GRADIENT TEXT
                    </span>
                  </div>
                </div>
              </section>

              {/* Foundation Surfaces */}
              <section id="foundation-surfaces" className="space-y-6">
                <div className="flex items-center gap-3">
                  <Layers className="w-5 h-5 text-primary" />
                  <h2 className="text-xl font-black text-on-surface tracking-tight">Foundation Surfaces</h2>
                </div>
                
                <div className="relative h-[500px] rounded-3xl overflow-hidden bg-surface-container flex items-end p-12 gap-8">
                  <img 
                    src="https://picsum.photos/seed/vibrant/1920/1080?blur=4" 
                    alt="Background" 
                    className="absolute inset-0 w-full h-full object-cover opacity-40"
                    referrerPolicy="no-referrer"
                  />
                  
                  <div className="relative z-10 w-64 h-80 bg-surface-container-lowest/40 backdrop-blur-xl border border-white/30 rounded-2xl p-6 shadow-2xl shadow-blue-900/10">
                    <p className="text-[10px] font-black text-on-surface/60 uppercase tracking-widest">wx-glass-card</p>
                    <div className="mt-8 space-y-4">
                      <div className="h-2 w-full bg-white/40 rounded-full" />
                      <div className="h-2 w-2/3 bg-white/40 rounded-full" />
                    </div>
                  </div>

                  <div className="relative z-10 w-64 h-64 bg-surface-container-lowest/20 backdrop-blur-md border border-white/20 rounded-2xl p-6 shadow-xl shadow-blue-900/5">
                    <p className="text-[10px] font-black text-on-surface/40 uppercase tracking-widest">wx-glass-card-light</p>
                    <div className="mt-8 flex items-center gap-4">
                      <div className="w-10 h-10 rounded-full bg-white/30" />
                      <div className="h-2 flex-1 bg-white/20 rounded-full" />
                    </div>
                  </div>

                  <div className="absolute bottom-12 right-12 z-10 w-96 h-14 bg-surface-container-lowest/60 backdrop-blur-2xl border border-white/40 rounded-full px-6 flex items-center gap-4 shadow-2xl shadow-blue-900/10">
                    <Search className="w-5 h-5 text-on-surface-variant opacity-60" />
                    <span className="text-sm font-bold text-on-surface-variant opacity-60">wx-glass-toolbar</span>
                  </div>
                </div>
              </section>

              {/* Density Modes */}
              <section id="density-modes" className="bg-surface-container-lowest rounded-2xl p-10 shadow-sm border border-outline-variant space-y-8 transition-colors duration-300">
                <div className="flex items-center justify-between">
                  <div className="flex items-center gap-3">
                    <Menu className="w-5 h-5 text-primary" />
                    <h2 className="text-xl font-black text-on-surface tracking-tight">Density Modes</h2>
                  </div>
                  <p className="text-xs text-slate-400 font-medium italic">3 density modes: compact | default | comfortable</p>
                </div>

                <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                  <div 
                    className="p-8 rounded-2xl bg-amber-50/50 border border-amber-100/50 space-y-2 cursor-pointer hover:scale-105 transition-transform"
                  >
                    <h4 className="font-bold text-amber-900">Compact</h4>
                    <p className="text-[11px] text-amber-700/80 font-medium">Input: 28px, Row: 32px, Font: 11px</p>
                  </div>
                  <div 
                    className="p-8 rounded-2xl bg-blue-50/50 border border-blue-200 ring-4 ring-blue-500/5 space-y-2 relative cursor-pointer hover:scale-105 transition-transform"
                  >
                    <div className="flex justify-between items-center">
                      <h4 className="font-bold text-blue-900">Default</h4>
                      <CheckCircle2 className="w-4 h-4 text-blue-600" />
                    </div>
                    <p className="text-[11px] text-blue-700/80 font-medium">Input: 36px, Row: 40px, Font: 14px</p>
                  </div>
                  <div 
                    className="p-8 rounded-2xl bg-emerald-50/50 border border-emerald-100/50 space-y-2 cursor-pointer hover:scale-105 transition-transform"
                  >
                    <h4 className="font-bold text-emerald-900">Comfortable</h4>
                    <p className="text-[11px] text-emerald-700/80 font-medium">Input: 44px, Row: 48px, Font: 15px</p>
                  </div>
                </div>
              </section>
            </div>
          </div>
        )}
      </main>

      {/* MODAL OVERLAY */}
      <AnimatePresence>
        {modalConfig?.isOpen && (
          <div className="fixed inset-0 z-[100] flex items-center justify-center p-4">
            <motion.div 
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              exit={{ opacity: 0 }}
              onClick={() => setModalConfig(null)}
              className="absolute inset-0 bg-slate-900/40 backdrop-blur-sm"
            />
            <motion.div 
              initial={{ opacity: 0, scale: 0.9, y: 20 }}
              animate={{ opacity: 1, scale: 1, y: 0 }}
              exit={{ opacity: 0, scale: 0.9, y: 20 }}
              transition={{ type: 'spring', damping: 25, stiffness: 300 }}
              className={`relative bg-white rounded-[24px] shadow-2xl w-full ${modalConfig.type === 'config' ? 'max-w-[900px]' : modalConfig.type === 'campaign-form' ? 'max-w-[650px]' : modalConfig.type === 'system-info' ? 'max-w-[550px]' : modalConfig.type === 'result' ? 'max-w-[500px]' : 'max-w-[340px]'} overflow-hidden border border-slate-100 ${modalConfig.type === 'sidebar' ? 'max-w-sm mr-0 ml-auto h-full rounded-none' : ''}`}
            >
              {modalConfig.type === 'system-info' ? (
                <div className="flex flex-col w-full">
                  {/* Header */}
                  <div className="bg-gradient-to-r from-[#00c6ff] to-[#0072ff] px-6 py-5 flex items-center justify-between">
                    <div className="flex items-center space-x-4">
                      <div className="w-12 h-12 rounded-2xl bg-white/20 flex items-center justify-center text-white backdrop-blur-md">
                        <Info className="w-7 h-7" />
                      </div>
                      <h3 className="text-2xl font-bold text-white tracking-tight">{modalConfig.title}</h3>
                    </div>
                    <button 
                      onClick={() => setModalConfig(null)}
                      className="w-10 h-10 rounded-full flex items-center justify-center text-white/60 hover:bg-white/10 hover:text-white transition-all"
                    >
                      <X className="w-6 h-6" />
                    </button>
                  </div>

                  {/* Body */}
                  <div className="p-8 space-y-6">
                    <p className="text-slate-600 font-medium leading-relaxed">
                      {modalConfig.message}
                    </p>
                    
                    <div className="bg-blue-50/50 border border-blue-100 p-6 rounded-[24px] flex items-start space-x-4">
                      <div className="w-10 h-10 rounded-xl bg-white flex items-center justify-center text-blue-500 shadow-sm shrink-0">
                        <Info className="w-5 h-5" />
                      </div>
                      <div className="space-y-1">
                        <h4 className="font-bold text-blue-900">Tính năng mới</h4>
                        <p className="text-sm text-blue-700/80 font-medium leading-relaxed">
                          Dashboard analytics, Bulk import, Multi-proxy support.
                        </p>
                      </div>
                    </div>
                  </div>

                  {/* Footer */}
                  <div className="px-8 py-6 flex items-center justify-end">
                    <button 
                      onClick={() => setModalConfig(null)}
                      className="px-8 py-3 bg-gradient-to-r from-[#00c6ff] to-[#0072ff] text-white rounded-2xl text-sm font-bold shadow-lg shadow-blue-500/20 hover:scale-105 transition-all active:scale-95"
                    >
                      Đã hiểu
                    </button>
                  </div>
                </div>
              ) : modalConfig.type === 'campaign-form' ? (
                <div className="flex flex-col w-full">
                  {/* Header */}
                  <div className="px-8 py-6 flex items-center justify-between border-b border-slate-100">
                    <div>
                      <h3 className="text-2xl font-bold text-slate-900 tracking-tight">{modalConfig.title}</h3>
                      <p className="text-slate-500 text-sm font-medium mt-1">{modalConfig.message}</p>
                    </div>
                    <button 
                      onClick={() => setModalConfig(null)}
                      className="w-10 h-10 rounded-full flex items-center justify-center text-slate-400 hover:bg-slate-100 hover:text-slate-600 transition-all"
                    >
                      <X className="w-6 h-6" />
                    </button>
                  </div>

                  {/* Body */}
                  <div className="p-8 space-y-6">
                    {/* Tên chiến dịch */}
                    <div className="space-y-2">
                      <label className="text-[11px] font-bold text-slate-400 uppercase tracking-widest">Tên chiến dịch</label>
                      <input 
                        type="text"
                        value={campaignName}
                        onChange={(e) => setCampaignName(e.target.value)}
                        placeholder="VD: Campaign_Summer_2026..."
                        className="w-full px-5 py-3.5 bg-white border border-slate-200 rounded-2xl text-sm font-medium focus:outline-none focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all"
                      />
                    </div>

                    <div className="grid grid-cols-2 gap-6">
                      {/* Phương thức */}
                      <div className="space-y-2">
                        <label className="text-[11px] font-bold text-slate-400 uppercase tracking-widest">Phương thức</label>
                        <div className="relative">
                          <select 
                            value={campaignMethod}
                            onChange={(e) => setCampaignMethod(e.target.value)}
                            className="w-full px-5 py-3.5 bg-white border border-slate-200 rounded-2xl text-sm font-medium focus:outline-none focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all appearance-none"
                          >
                            <option value="" disabled>Chọn...</option>
                            <option value="direct">Trực tiếp</option>
                            <option value="indirect">Gián tiếp</option>
                            <option value="social">Mạng xã hội</option>
                          </select>
                          <ChevronDown className="absolute right-4 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-400 pointer-events-none" />
                        </div>
                      </div>

                      {/* Ngày bắt đầu */}
                      <div className="space-y-2">
                        <label className="text-[11px] font-bold text-slate-400 uppercase tracking-widest">Ngày bắt đầu</label>
                        <div className="relative">
                          <input 
                            type="date"
                            value={campaignStartDate}
                            onChange={(e) => setCampaignStartDate(e.target.value)}
                            className="w-full px-5 py-3.5 bg-white border border-slate-200 rounded-2xl text-sm font-medium focus:outline-none focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all"
                          />
                        </div>
                      </div>
                    </div>

                    {/* Ngân sách */}
                    <div className="space-y-4">
                      <div className="flex items-center justify-between">
                        <label className="text-[11px] font-bold text-slate-400 uppercase tracking-widest">Ngân sách (%)</label>
                        <span className="text-sm font-bold text-primary">{campaignBudget}</span>
                      </div>
                      <div className="flex items-center space-x-4">
                        <span className="text-xs font-bold text-slate-300">0</span>
                        <input 
                          type="range"
                          min="0"
                          max="100"
                          step="5"
                          value={campaignBudget}
                          onChange={(e) => setCampaignBudget(parseInt(e.target.value))}
                          className="flex-1 h-2 bg-slate-100 rounded-lg appearance-none cursor-pointer accent-primary"
                        />
                        <span className="text-xs font-bold text-slate-300">100</span>
                      </div>
                    </div>

                    {/* Ghi chú */}
                    <div className="space-y-2">
                      <label className="text-[11px] font-bold text-slate-400 uppercase tracking-widest">Ghi chú</label>
                      <textarea 
                        value={campaignNotes}
                        onChange={(e) => setCampaignNotes(e.target.value)}
                        placeholder="Nhập ghi chú cho chiến dịch..."
                        rows={3}
                        className="w-full px-5 py-3.5 bg-white border border-slate-200 rounded-2xl text-sm font-medium focus:outline-none focus:ring-2 focus:ring-primary/20 focus:border-primary transition-all resize-none"
                      />
                    </div>
                  </div>

                  {/* Footer */}
                  <div className="px-8 py-6 bg-slate-50/50 border-t border-slate-100 flex items-center justify-end space-x-4">
                    <button 
                      onClick={() => setModalConfig(null)}
                      className="px-6 py-2.5 text-slate-500 hover:text-slate-700 font-bold text-sm transition-colors"
                    >
                      Hủy
                    </button>
                    <button 
                      onClick={() => {
                        addToast('success', 'Thành công', 'Đã tạo chiến dịch mới: ' + (campaignName || 'Unnamed'));
                        setModalConfig(null);
                        // Reset form
                        setCampaignName('');
                        setCampaignMethod('');
                        setCampaignStartDate('');
                        setCampaignBudget(40);
                        setCampaignNotes('');
                      }}
                      className="px-8 py-3 bg-gradient-to-r from-[#00c6ff] to-[#0072ff] text-white rounded-2xl text-sm font-bold shadow-lg shadow-blue-500/20 flex items-center space-x-2 hover:scale-105 transition-all active:scale-95"
                    >
                      <Plus className="w-5 h-5" />
                      <span>Tạo chiến dịch</span>
                    </button>
                  </div>
                </div>
              ) : modalConfig.type === 'config' ? (
                <div className="flex flex-col h-[600px] w-full max-h-[90vh]">
                  {/* Header */}
                  <div className="bg-gradient-to-r from-[#00c6ff] to-[#0072ff] px-6 py-4 flex items-center space-x-4 shrink-0">
                    <div className="w-10 h-10 rounded-xl bg-white/20 flex items-center justify-center text-white">
                      <Settings className="w-6 h-6" />
                    </div>
                    <div>
                      <h3 className="text-xl font-bold text-white tracking-tight leading-tight">Cấu hình chạy</h3>
                      <p className="text-white/80 text-sm font-medium">Proxy và đồng thời</p>
                    </div>
                  </div>

                  <div className="flex flex-1 overflow-hidden">
                    {/* Sidebar */}
                    <div className="w-64 border-r border-slate-100 bg-slate-50/30 p-4 space-y-2 shrink-0">
                      <button 
                        onClick={() => setActiveConfigTab('proxy')}
                        className={`w-full flex items-center space-x-3 p-3 rounded-2xl transition-all ${activeConfigTab === 'proxy' ? 'bg-white shadow-sm border border-slate-100 text-primary' : 'text-slate-500 hover:bg-slate-100/50'}`}
                      >
                        <div className={`w-10 h-10 rounded-xl flex items-center justify-center ${activeConfigTab === 'proxy' ? 'bg-primary/10 text-primary' : 'bg-slate-200/50 text-slate-400'}`}>
                          <Globe className="w-5 h-5" />
                        </div>
                        <div className="text-left">
                          <p className={`text-sm font-bold ${activeConfigTab === 'proxy' ? 'text-slate-900' : 'text-slate-600'}`}>Proxy</p>
                          <p className="text-[10px] font-medium opacity-60">Cấu hình proxy</p>
                        </div>
                        {activeConfigTab === 'proxy' && <ChevronRightIcon className="w-4 h-4 ml-auto opacity-40" />}
                      </button>

                      <button 
                        onClick={() => setActiveConfigTab('chung')}
                        className={`w-full flex items-center space-x-3 p-3 rounded-2xl transition-all ${activeConfigTab === 'chung' ? 'bg-white shadow-sm border border-slate-100 text-primary' : 'text-slate-500 hover:bg-slate-100/50'}`}
                      >
                        <div className={`w-10 h-10 rounded-xl flex items-center justify-center ${activeConfigTab === 'chung' ? 'bg-purple-100 text-purple-600' : 'bg-slate-200/50 text-slate-400'}`}>
                          <Settings2 className="w-5 h-5" />
                        </div>
                        <div className="text-left">
                          <p className={`text-sm font-bold ${activeConfigTab === 'chung' ? 'text-slate-900' : 'text-slate-600'}`}>Chung</p>
                          <p className="text-[10px] font-medium opacity-60">Cài đặt chung</p>
                        </div>
                        {activeConfigTab === 'chung' && <ChevronRightIcon className="w-4 h-4 ml-auto opacity-40" />}
                      </button>

                      <button 
                        onClick={() => setActiveConfigTab('nang-cao')}
                        className={`w-full flex items-center space-x-3 p-3 rounded-2xl transition-all ${activeConfigTab === 'nang-cao' ? 'bg-white shadow-sm border border-slate-100 text-primary' : 'text-slate-500 hover:bg-slate-100/50'}`}
                      >
                        <div className={`w-10 h-10 rounded-xl flex items-center justify-center ${activeConfigTab === 'nang-cao' ? 'bg-orange-100 text-orange-600' : 'bg-slate-200/50 text-slate-400'}`}>
                          <Sliders className="w-5 h-5" />
                        </div>
                        <div className="text-left">
                          <p className={`text-sm font-bold ${activeConfigTab === 'nang-cao' ? 'text-slate-900' : 'text-slate-600'}`}>Nâng cao</p>
                          <p className="text-[10px] font-medium opacity-60">Tuỳ chọn nâng cao</p>
                        </div>
                        {activeConfigTab === 'nang-cao' && <ChevronRightIcon className="w-4 h-4 ml-auto opacity-40" />}
                      </button>
                    </div>

                    {/* Content Area */}
                    <div className="flex-1 overflow-y-auto p-8 bg-white">
                      {activeConfigTab === 'proxy' && (
                        <div className="space-y-8 animate-in fade-in slide-in-from-right-4 duration-300">
                          <div className="flex items-center space-x-4">
                            <div className="w-14 h-14 rounded-2xl bg-blue-500 flex items-center justify-center text-white shadow-lg shadow-blue-500/20">
                              <Globe className="w-8 h-8" />
                            </div>
                            <div>
                              <h4 className="text-2xl font-bold text-slate-900 tracking-tight">Proxy</h4>
                              <p className="text-slate-500 font-medium">Cấu hình proxy</p>
                            </div>
                          </div>

                          <div className="space-y-6">
                            <div className="space-y-2">
                              <label className="text-xs font-bold text-slate-400 uppercase tracking-wider">Proxy URL</label>
                              <div className="relative group">
                                <div className="absolute left-4 top-1/2 -translate-y-1/2 text-slate-400 group-focus-within:text-primary transition-colors">
                                  <Globe className="w-5 h-5" />
                                </div>
                                <input 
                                  type="text"
                                  value={proxyUrl}
                                  onChange={(e) => setProxyUrl(e.target.value)}
                                  placeholder="http://proxy:port"
                                  className="w-full pl-12 pr-4 py-3.5 bg-slate-50 border border-slate-100 rounded-2xl text-sm font-medium focus:outline-none focus:ring-2 focus:ring-primary/20 focus:bg-white transition-all"
                                />
                              </div>
                            </div>

                            <div className="space-y-2">
                              <label className="text-xs font-bold text-slate-400 uppercase tracking-wider">Username</label>
                              <div className="relative group">
                                <div className="absolute left-4 top-1/2 -translate-y-1/2 text-slate-400 group-focus-within:text-primary transition-colors">
                                  <User className="w-5 h-5" />
                                </div>
                                <input 
                                  type="text"
                                  value={proxyUsername}
                                  onChange={(e) => setProxyUsername(e.target.value)}
                                  placeholder="proxy_user"
                                  className="w-full pl-12 pr-4 py-3.5 bg-slate-50 border border-slate-100 rounded-2xl text-sm font-medium focus:outline-none focus:ring-2 focus:ring-primary/20 focus:bg-white transition-all"
                                />
                              </div>
                            </div>

                            <div className="space-y-2">
                              <label className="text-xs font-bold text-slate-400 uppercase tracking-wider">Password</label>
                              <div className="relative group">
                                <div className="absolute left-4 top-1/2 -translate-y-1/2 text-slate-400 group-focus-within:text-primary transition-colors">
                                  <Lock className="w-5 h-5" />
                                </div>
                                <input 
                                  type={showPassword ? "text" : "password"}
                                  value={proxyPassword}
                                  onChange={(e) => setProxyPassword(e.target.value)}
                                  placeholder="••••••"
                                  className="w-full pl-12 pr-12 py-3.5 bg-slate-50 border border-slate-100 rounded-2xl text-sm font-medium focus:outline-none focus:ring-2 focus:ring-primary/20 focus:bg-white transition-all"
                                />
                                <button 
                                  onClick={() => setShowPassword(!showPassword)}
                                  className="absolute right-4 top-1/2 -translate-y-1/2 text-slate-400 hover:text-slate-600 transition-colors"
                                >
                                  {showPassword ? <EyeOff className="w-5 h-5" /> : <Eye className="w-5 h-5" />}
                                </button>
                              </div>
                            </div>
                          </div>
                        </div>
                      )}

                      {activeConfigTab === 'chung' && (
                        <div className="space-y-8 animate-in fade-in slide-in-from-right-4 duration-300">
                          <div className="flex items-center space-x-4">
                            <div className="w-14 h-14 rounded-2xl bg-purple-500 flex items-center justify-center text-white shadow-lg shadow-purple-500/20">
                              <Settings2 className="w-8 h-8" />
                            </div>
                            <div>
                              <h4 className="text-2xl font-bold text-slate-900 tracking-tight">Chung</h4>
                              <p className="text-slate-500 font-medium">Cài đặt chung</p>
                            </div>
                          </div>

                          <div className="space-y-4">
                            <div className="flex items-center justify-between p-4 bg-slate-50/50 rounded-2xl border border-slate-100">
                              <div className="flex items-center space-x-4">
                                <div className="w-10 h-10 rounded-xl bg-white flex items-center justify-center text-primary shadow-sm">
                                  <RefreshCw className="w-5 h-5" />
                                </div>
                                <div>
                                  <p className="text-sm font-bold text-slate-900">Tự động retry</p>
                                  <p className="text-xs text-slate-500 font-medium">Thử lại khi gặp lỗi</p>
                                </div>
                              </div>
                              <button 
                                onClick={() => setAutoRetry(!autoRetry)}
                                className={`w-12 h-6 rounded-full transition-all relative ${autoRetry ? 'bg-primary' : 'bg-slate-300'}`}
                              >
                                <div className={`absolute top-1 w-4 h-4 bg-white rounded-full transition-all ${autoRetry ? 'right-1' : 'left-1'}`} />
                              </button>
                            </div>

                            <div className="flex items-center justify-between p-4 bg-slate-50/50 rounded-2xl border border-slate-100">
                              <div className="flex items-center space-x-4">
                                <div className="w-10 h-10 rounded-xl bg-white flex items-center justify-center text-blue-500 shadow-sm">
                                  <FileText className="w-5 h-5" />
                                </div>
                                <div>
                                  <p className="text-sm font-bold text-slate-900">Log chi tiết</p>
                                  <p className="text-xs text-slate-500 font-medium">Ghi log đầy đủ</p>
                                </div>
                              </div>
                              <button 
                                onClick={() => setDetailedLog(!detailedLog)}
                                className={`w-12 h-6 rounded-full transition-all relative ${detailedLog ? 'bg-primary' : 'bg-slate-300'}`}
                              >
                                <div className={`absolute top-1 w-4 h-4 bg-white rounded-full transition-all ${detailedLog ? 'right-1' : 'left-1'}`} />
                              </button>
                            </div>
                          </div>
                        </div>
                      )}

                      {activeConfigTab === 'nang-cao' && (
                        <div className="space-y-8 animate-in fade-in slide-in-from-right-4 duration-300">
                          <div className="flex items-center space-x-4">
                            <div className="w-14 h-14 rounded-2xl bg-orange-500 flex items-center justify-center text-white shadow-lg shadow-orange-500/20">
                              <Sliders className="w-8 h-8" />
                            </div>
                            <div>
                              <h4 className="text-2xl font-bold text-slate-900 tracking-tight">Nâng cao</h4>
                              <p className="text-slate-500 font-medium">Tuỳ chọn nâng cao</p>
                            </div>
                          </div>

                          <div className="space-y-4">
                            <h5 className="text-xs font-bold text-primary uppercase tracking-widest">TÙY CHỌN NÂNG CAO</h5>
                            
                            <button 
                              onClick={() => setSelectedMode(1)}
                              className={`w-full flex items-center space-x-4 p-5 rounded-2xl border transition-all text-left ${selectedMode === 1 ? 'bg-blue-50/50 border-primary ring-1 ring-primary' : 'bg-white border-slate-100 hover:border-slate-200'}`}
                            >
                              <div className={`w-6 h-6 rounded-full border-2 flex items-center justify-center transition-all ${selectedMode === 1 ? 'border-primary' : 'border-slate-300'}`}>
                                {selectedMode === 1 && <div className="w-3 h-3 bg-primary rounded-full" />}
                              </div>
                              <div>
                                <p className={`font-bold ${selectedMode === 1 ? 'text-primary' : 'text-slate-900'}`}>Chế độ 1</p>
                                <p className="text-sm text-slate-500 font-medium">Mặc định</p>
                              </div>
                            </button>

                            <button 
                              onClick={() => setSelectedMode(2)}
                              className={`w-full flex items-center space-x-4 p-5 rounded-2xl border transition-all text-left ${selectedMode === 2 ? 'bg-blue-50/50 border-primary ring-1 ring-primary' : 'bg-white border-slate-100 hover:border-slate-200'}`}
                            >
                              <div className={`w-6 h-6 rounded-full border-2 flex items-center justify-center transition-all ${selectedMode === 2 ? 'border-primary' : 'border-slate-300'}`}>
                                {selectedMode === 2 && <div className="w-3 h-3 bg-primary rounded-full" />}
                              </div>
                              <div className="flex-1">
                                <p className={`font-bold ${selectedMode === 2 ? 'text-primary' : 'text-slate-900'}`}>Chế độ 2</p>
                                <p className="text-sm text-slate-500 font-medium">Nâng cao</p>
                              </div>
                              <div className="px-2 py-1 bg-slate-100 rounded text-[10px] font-bold text-slate-500 uppercase tracking-tighter">Pro</div>
                            </button>
                          </div>
                        </div>
                      )}
                    </div>
                  </div>

                  {/* Footer */}
                  <div className="px-8 py-5 bg-slate-50/50 border-t border-slate-100 flex items-center justify-end space-x-4 shrink-0">
                    <button 
                      onClick={() => setModalConfig(null)}
                      className="px-6 py-2.5 flex items-center space-x-2 text-slate-500 hover:text-slate-700 font-bold text-sm transition-colors"
                    >
                      <X className="w-4 h-4" />
                      <span>Hủy</span>
                    </button>
                    <button 
                      onClick={() => {
                        addToast('success', 'Thành công', 'Đã lưu cấu hình hệ thống');
                        setModalConfig(null);
                      }}
                      className="px-8 py-3 bg-gradient-to-r from-[#00c6ff] to-[#0072ff] text-white rounded-2xl text-sm font-bold shadow-lg shadow-blue-500/20 flex items-center space-x-2 hover:scale-105 transition-all active:scale-95"
                    >
                      <Check className="w-5 h-5" />
                      <span>Lưu thay đổi</span>
                    </button>
                  </div>
                </div>
              ) : modalConfig.type === 'result' ? (
                <>
                  {/* Result Header */}
                  <div className="bg-gradient-to-r from-[#00c6ff] to-[#0072ff] px-6 py-5 flex items-center space-x-4">
                    <div className="w-10 h-10 rounded-xl bg-white/20 flex items-center justify-center text-white">
                      <List className="w-6 h-6" />
                    </div>
                    <div>
                      <h3 className="text-xl font-bold text-white tracking-tight leading-tight">{modalConfig.title}</h3>
                      <p className="text-white/80 text-sm font-medium">{modalConfig.message}</p>
                    </div>
                  </div>

                  {/* Result Body */}
                  <div className="p-6 space-y-4 max-h-[400px] overflow-y-auto">
                    {isRechecking ? (
                      <div className="py-12 flex flex-col items-center justify-center space-y-4">
                        <motion.div
                          animate={{ rotate: 360 }}
                          transition={{ repeat: Infinity, duration: 1, ease: "linear" }}
                        >
                          <RefreshCw className="w-10 h-10 text-primary" />
                        </motion.div>
                        <p className="text-slate-500 font-medium animate-pulse">Đang kiểm tra lại...</p>
                      </div>
                    ) : (
                      <>
                        {/* Success Item */}
                        <div className="flex items-start space-x-3 p-4 bg-emerald-50 border border-emerald-100 rounded-2xl">
                          <div className="mt-0.5 w-6 h-6 rounded-full bg-emerald-500 flex items-center justify-center text-white flex-shrink-0">
                            <Check className="w-4 h-4" />
                          </div>
                          <div className="space-y-1">
                            <p className="text-emerald-900 font-bold text-sm">BM: 1234567890 - Active</p>
                            <p className="text-emerald-700 text-xs font-medium">Trạng thái tài khoản ổn định, sẵn sàng sử dụng.</p>
                          </div>
                        </div>

                        {/* Success Item 2 */}
                        <div className="flex items-start space-x-3 p-4 bg-emerald-50 border border-emerald-100 rounded-2xl">
                          <div className="mt-0.5 w-6 h-6 rounded-full bg-emerald-500 flex items-center justify-center text-white flex-shrink-0">
                            <Check className="w-4 h-4" />
                          </div>
                          <div className="space-y-1">
                            <p className="text-emerald-900 font-bold text-sm">BM: 9876543210 - Verified</p>
                            <p className="text-emerald-700 text-xs font-medium">Xác minh doanh nghiệp hoàn tất.</p>
                          </div>
                        </div>

                        {/* Error Item */}
                        <div className="flex items-start space-x-3 p-4 bg-red-50 border border-red-100 rounded-2xl">
                          <div className="mt-0.5 w-6 h-6 rounded-full bg-red-500 flex items-center justify-center text-white flex-shrink-0">
                            <X className="w-4 h-4" />
                          </div>
                          <div className="space-y-1">
                            <p className="text-red-900 font-bold text-sm">BM: 5556667778 - Restricted</p>
                            <p className="text-red-700 text-xs font-medium">Tài khoản bị hạn chế quảng cáo. Vui lòng kiểm tra lại.</p>
                          </div>
                        </div>
                      </>
                    )}
                  </div>

                  {/* Result Footer */}
                  <div className="px-6 py-4 bg-slate-50/50 border-t border-slate-100 flex items-center justify-between">
                    <button 
                      onClick={() => setModalConfig(null)}
                      className="px-6 py-2.5 text-slate-500 hover:text-slate-700 font-bold text-sm transition-colors"
                    >
                      Đóng
                    </button>
                    <button 
                      onClick={handleRecheck}
                      disabled={isRechecking}
                      className="px-6 py-2.5 bg-gradient-to-r from-[#00c6ff] to-[#0072ff] text-white rounded-xl text-sm font-bold shadow-lg shadow-blue-500/20 flex items-center space-x-2 hover:scale-105 transition-all active:scale-95 disabled:opacity-50 disabled:scale-100"
                    >
                      <RefreshCw className={`w-4 h-4 ${isRechecking ? 'animate-spin' : ''}`} />
                      <span>Kiểm tra lại</span>
                    </button>
                  </div>
                </>
              ) : (
                <>
                  {/* Modal Header with Dynamic Color */}
                  <div className={`px-5 py-2.5 flex items-center space-x-3 ${
                    modalConfig.type === 'info' ? 'bg-gradient-to-r from-[#00c6ff] to-[#0072ff]' :
                    modalConfig.type === 'success' ? 'bg-[#10b981]' :
                    modalConfig.type === 'error' ? 'bg-[#ef4444]' :
                    modalConfig.type === 'question' ? 'bg-[#06b6d4]' :
                    modalConfig.type === 'delete' ? 'bg-[#ef4444]' :
                    modalConfig.type === 'warning' ? 'bg-[#f59e0b]' :
                    'bg-[#06b6d4]'
                  }`}>
                    <div className="w-7 h-7 rounded-lg bg-white/20 flex items-center justify-center text-white">
                      {modalConfig.type === 'info' && <Info className="w-4 h-4" />}
                      {modalConfig.type === 'success' && <CheckCircle2 className="w-4 h-4" />}
                      {modalConfig.type === 'error' && <X className="w-4 h-4" />}
                      {modalConfig.type === 'question' && <HelpCircle className="w-4 h-4" />}
                      {modalConfig.type === 'delete' && <Trash2 className="w-4 h-4" />}
                      {modalConfig.type === 'warning' && <AlertTriangle className="w-4 h-4" />}
                      {modalConfig.type === 'default' && <ExternalLink className="w-4 h-4" />}
                      {modalConfig.type === 'sidebar' && <Settings className="w-4 h-4" />}
                    </div>
                    <h3 className="text-base font-bold text-white tracking-tight">{modalConfig.title}</h3>
                  </div>

                  <div className="px-5 py-4 space-y-4">
                    <p className="text-sm text-slate-600 font-medium leading-relaxed">
                      {modalConfig.message}
                    </p>

                    {/* Separator line */}
                    <div className="h-px bg-slate-100 w-full" />

                    <div className="flex items-center justify-end">
                      <button 
                        onClick={() => setModalConfig(null)}
                        className={`px-6 py-2 rounded-xl text-xs font-bold text-white shadow-lg transition-all hover:scale-105 active:scale-95 ${
                          modalConfig.type === 'info' ? 'bg-gradient-to-r from-[#00c6ff] to-[#0072ff] shadow-blue-500/20' :
                          modalConfig.type === 'success' ? 'bg-[#10b981] shadow-emerald-500/20' :
                          modalConfig.type === 'error' ? 'bg-[#ef4444] shadow-red-500/20' :
                          modalConfig.type === 'question' ? 'bg-[#06b6d4] shadow-cyan-500/20' :
                          modalConfig.type === 'delete' ? 'bg-[#ef4444] shadow-red-500/20' :
                          modalConfig.type === 'warning' ? 'bg-[#f59e0b] shadow-orange-500/20' :
                          'bg-[#06b6d4] shadow-cyan-500/20'
                        }`}
                      >
                        Đồng ý
                      </button>
                    </div>
                  </div>
                </>
              )}
            </motion.div>
          </div>
        )}
      </AnimatePresence>

      <AnimatePresence>
        {isDrawerOpen && (
          <div className="fixed inset-0 z-[100] flex justify-end overflow-hidden">
            {/* Overlay */}
            <motion.div 
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              exit={{ opacity: 0 }}
              onClick={() => setIsDrawerOpen(false)}
              className="absolute inset-0 bg-slate-900/40 backdrop-blur-[2px]"
            />
            
            {/* Drawer Panel */}
            <motion.div 
              initial={{ x: '100%' }}
              animate={{ x: 0 }}
              exit={{ x: '100%' }}
              transition={{ type: 'spring', damping: 25, stiffness: 200 }}
              className="relative w-full max-w-[420px] bg-white shadow-2xl flex flex-col h-full"
            >
              {/* Header */}
              <div className="flex items-center justify-between px-6 py-5 border-b border-slate-100">
                <h2 className="text-lg font-bold text-slate-800">Chi tiết tài khoản</h2>
                <button 
                  onClick={() => setIsDrawerOpen(false)}
                  className="p-2 hover:bg-slate-50 rounded-full text-slate-400 hover:text-slate-600 transition-colors"
                >
                  <X className="w-5 h-5" />
                </button>
              </div>

              {/* Content */}
              <div className="flex-1 overflow-y-auto p-6 space-y-8 no-scrollbar">
                {/* Account Info */}
                <div className="flex items-center space-x-4">
                  <div className="relative">
                    <div className="w-14 h-14 bg-sky-50 rounded-xl flex items-center justify-center border border-sky-100">
                      <div className="w-8 h-8 text-sky-500">
                        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
                          <path d="M4 15s1-1 4-1 5 2 8 2 4-1 4-1V3s-1 1-4 1-5-2-8-2-4 1-4 1z" />
                          <line x1="4" y1="22" x2="4" y2="15" />
                        </svg>
                      </div>
                    </div>
                    <div className="absolute -bottom-1 -right-1 w-4 h-4 bg-emerald-500 border-2 border-white rounded-full" />
                  </div>
                  <div>
                    <h3 className="text-base font-bold text-slate-800">TKQC_Campaign_A</h3>
                    <p className="text-xs text-slate-400 font-medium">ID: 123456789</p>
                  </div>
                </div>

                {/* Form Fields */}
                <div className="space-y-6">
                  {/* Campaign Name */}
                  <div className="space-y-2.5">
                    <label className="text-[10px] font-bold text-slate-400 uppercase tracking-widest">TÊN CHIẾN DỊCH</label>
                    <input 
                      type="text" 
                      value={drawerCampaignName}
                      onChange={(e) => setDrawerCampaignName(e.target.value)}
                      className="w-full px-4 py-3 bg-white border border-slate-100 rounded-xl text-sm text-slate-700 font-medium focus:outline-none focus:ring-2 focus:ring-sky-500/10 focus:border-sky-500 transition-all"
                    />
                  </div>

                  {/* Max Spend Slider */}
                  <div className="space-y-4">
                    <div className="flex items-center justify-between">
                      <label className="text-[10px] font-bold text-slate-400 uppercase tracking-widest">CHI TIÊU TỐI ĐA (%)</label>
                      <span className="text-sm font-bold text-sky-500">{drawerMaxSpend}</span>
                    </div>
                    <div className="relative pt-1">
                      <input 
                        type="range" 
                        min="0" 
                        max="100" 
                        step="5"
                        value={drawerMaxSpend}
                        onChange={(e) => setDrawerMaxSpend(parseInt(e.target.value))}
                        className="w-full h-1.5 bg-slate-100 rounded-lg appearance-none cursor-pointer accent-sky-500"
                      />
                      <div className="flex justify-between mt-2">
                        <span className="text-[10px] font-bold text-slate-300">0</span>
                        <span className="text-[10px] font-bold text-slate-300">100</span>
                      </div>
                    </div>
                  </div>

                  {/* Auto Optimize Toggle */}
                  <div className="flex items-center justify-between py-2">
                    <div className="space-y-0.5">
                      <h4 className="text-sm font-bold text-slate-800">Tự động tối ưu</h4>
                      <p className="text-xs text-slate-400 font-medium">Bật tối ưu ngân sách tự động</p>
                    </div>
                    <button 
                      onClick={() => setDrawerAutoOptimize(!drawerAutoOptimize)}
                      className={`w-11 h-6 rounded-full relative transition-all duration-300 ${drawerAutoOptimize ? 'bg-sky-500' : 'bg-slate-200'}`}
                    >
                      <motion.div 
                        animate={{ x: drawerAutoOptimize ? 22 : 2 }}
                        className="absolute top-1 w-4 h-4 bg-white rounded-full shadow-sm"
                      />
                    </button>
                  </div>
                </div>
              </div>

              {/* Footer */}
              <div className="p-6 border-t border-slate-100 flex items-center justify-end space-x-3 bg-white">
                <button 
                  onClick={() => setIsDrawerOpen(false)}
                  className="px-6 py-2.5 text-sm font-bold text-slate-500 hover:text-slate-700 transition-colors"
                >
                  Hủy
                </button>
                <button 
                  onClick={() => {
                    // Handle save logic here
                    setIsDrawerOpen(false);
                  }}
                  className="px-8 py-2.5 bg-sky-500 hover:bg-sky-600 text-white rounded-xl text-sm font-bold shadow-lg shadow-sky-500/20 transition-all active:scale-95"
                >
                  Lưu
                </button>
              </div>
            </motion.div>
          </div>
        )}
      </AnimatePresence>

      {/* FOOTER */}
      <footer className="bg-white border-t border-slate-200 mt-20 px-8 py-12">
        <div className="max-w-6xl mx-auto flex flex-col md:flex-row justify-between items-center space-y-8 md:space-y-0">
          <div className="flex flex-col items-center md:items-start space-y-4">
            <div className="flex items-center space-x-2 text-slate-800">
              <div className="w-8 h-8 rounded bg-primary flex items-center justify-center text-white shadow-sm">
                <Bolt className="w-5 h-5" />
              </div>
              <span className="font-black text-xl tracking-tighter uppercase">WEMAKE</span>
            </div>
            <p className="text-xs text-slate-400 font-medium">© 2023 Wemake. All rights reserved.</p>
          </div>
          
          <div className="flex flex-wrap justify-center gap-8 text-sm text-slate-500 font-bold">
            <a href="#" className="hover:text-primary transition-colors flex items-center space-x-1">
              <span>Tài liệu API</span>
              <ExternalLink className="w-3 h-3" />
            </a>
            <a href="#" className="hover:text-primary transition-colors flex items-center space-x-1">
              <span>Hỗ trợ</span>
              <HelpCircle className="w-3 h-3" />
            </a>
            <a href="#" className="hover:text-primary transition-colors flex items-center space-x-1">
              <span>Github</span>
              <Github className="w-3 h-3" />
            </a>
            <span className="text-slate-300 font-black">V2.4.0</span>
          </div>
        </div>
      </footer>

      {/* Toast Container */}
      <div className="fixed top-6 right-6 z-[9999] flex flex-col gap-3 pointer-events-none">
        <AnimatePresence>
          {toasts.map((toast) => (
            <motion.div
              key={toast.id}
              initial={{ opacity: 0, x: 50, scale: 0.9 }}
              animate={{ opacity: 1, x: 0, scale: 1 }}
              exit={{ opacity: 0, x: 20, scale: 0.95 }}
              transition={{ type: 'spring', damping: 20, stiffness: 300 }}
              className={`pointer-events-auto w-80 rounded-2xl shadow-xl border p-4 flex items-start space-x-3 ${
                toast.type === 'info' ? 'bg-white border-blue-100' :
                toast.type === 'success' ? 'bg-green-50 border-green-100' :
                toast.type === 'warning' ? 'bg-white border-orange-100' :
                'bg-white border-red-100'
              }`}
            >
              <div className={`mt-0.5 p-1.5 rounded-xl ${
                toast.type === 'info' ? 'bg-blue-500 text-white' :
                toast.type === 'success' ? 'bg-emerald-500 text-white' :
                toast.type === 'warning' ? 'bg-orange-500 text-white' :
                'bg-rose-500 text-white'
              }`}>
                {toast.type === 'info' && <Info className="w-5 h-5" />}
                {toast.type === 'success' && <CheckCircle2 className="w-5 h-5" />}
                {toast.type === 'warning' && <AlertTriangle className="w-5 h-5" />}
                {toast.type === 'error' && <X className="w-5 h-5" />}
              </div>
              <div className="flex-1 min-w-0">
                <p className="text-sm font-bold text-slate-800">{toast.title}</p>
                <p className="text-xs text-slate-500 mt-0.5 leading-relaxed">{toast.message}</p>
              </div>
              <button 
                onClick={() => removeToast(toast.id)}
                className="text-slate-400 hover:text-slate-600 transition-colors p-1"
              >
                <X className="w-4 h-4" />
              </button>
            </motion.div>
          ))}
        </AnimatePresence>
      </div>
    </div>
  );
}
